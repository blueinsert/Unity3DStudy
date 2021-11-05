#include "RVOSimulator.h"
#include "Obstacle.h"
#include "Agent.h"
#include "Goal.h"
#include "Roadmap.h"
#include "KDTree.h"

namespace RVO {

  //---------------------------------------------------------------
  //      Implementation of RVO Simulator
  //---------------------------------------------------------------

  RVOSimulator*  RVOSimulator::_pinstance = 0;
  
  //---------------------------------------------------------------

  RVOSimulator::RVOSimulator() {
    _globalTime = 0;
    _timeStep = 0;
    _roadmap = new Roadmap();
    _defaultAgent = new Agent();
    _allAtGoals = false;

    _agentDefaultsHaveBeenSet = false;
    _simulationHasBeenInitialized = false;

  }

  RVOSimulator::~RVOSimulator() {
    delete _roadmap;
    delete _defaultAgent;
    for (int i = 0; i < (int) _agents.size(); ++i) {
      delete _agents[i];
    }
    for (int i = 0; i < (int) _obstacles.size(); ++i) {
      delete _obstacles[i];
    }
    for (int i = 0; i < (int) _goals.size(); ++i) {
      delete _goals[i];
    }
  }

  //---------------------------------------------------------------

  RVOSimulator* RVOSimulator::Instance() {
    if (_pinstance == NULL)  {  
      srand((unsigned int) time(NULL));
      _pinstance = new RVOSimulator(); // create sole instance
    }
    return _pinstance; // address of sole instance
  }

  //---------------------------------------------------------------

  int RVOSimulator::doStep() {
    if ( ! _simulationHasBeenInitialized ) {
      std::cerr << "Simulation is not initialized when attempt is made to do a step." << std::endl;
      return RVO_SIMULATION_NOT_INITIALIZED_WHEN_DOING_STEP;
    }
    if ( _timeStep == 0 ) {
      std::cerr << "Time step has not been set when attempt is made to do a step." << std::endl;
      return RVO_TIME_STEP_NOT_SET_WHEN_DOING_STEP;
    }
    
    _allAtGoals = true;

    // compute new velocities for agents
    _kdTree->buildAgentTree();
    
  #pragma omp parallel for
    for (int i = 0; i < (int)_agents.size(); ++i) {
      _agents[i]->computePreferredVelocity();  // alters _allAtGoals
      _agents[i]->computeNeighbors();
      _agents[i]->computeNewVelocity();
    }

    // update positions and velocity of _agents
  #pragma omp parallel for
    for (int i = 0; i < (int)_agents.size(); ++i) {
      _agents[i]->update();
    }

    _globalTime += _timeStep;

    return RVO_SUCCESS;
  }

  //----------------------------------------------------------------------------------
 
  int RVOSimulator::getNumAgents() const { return (int) _agents.size(); }
  bool RVOSimulator::getAgentReachedGoal(int i) const { return _agents[i]->_atGoal; }
  const Vector2& RVOSimulator::getAgentPosition(int i) const { return _agents[i]->_p; }
  void RVOSimulator::setAgentPosition(int i, const Vector2& p) { _agents[i]->_p = p; }
  const Vector2& RVOSimulator::getAgentVelocity(int i) const { return _agents[i]->_v; }
  void RVOSimulator::setAgentVelocity(int i, const Vector2& v) { _agents[i]->_v = v; }
  float RVOSimulator::getAgentRadius(int i) const { return _agents[i]->_r; }
  void RVOSimulator::setAgentRadius(int i, float r) { _agents[i]->_r = r; }
  int RVOSimulator::getAgentVelSampleCount(int i) const { return _agents[i]->_velSampleCount; }
  void RVOSimulator::setAgentVelSampleCount(int i, int samples) { _agents[i]->_velSampleCount = samples; }
  float RVOSimulator::getAgentNeighborDist(int i) const { return _agents[i]->_neighborDist; }
  void RVOSimulator::setAgentNeighborDist(int i, float distance) { _agents[i]->_neighborDist = distance; }
  int RVOSimulator::getAgentMaxNeighbors(int i) const { return _agents[i]->_maxNeighbors; }
  void RVOSimulator::setAgentMaxNeighbors(int i, int maximum) { _agents[i]->_maxNeighbors = maximum; }
  int RVOSimulator::getAgentClass(int i) const { return _agents[i]->_class; }
  void RVOSimulator::setAgentClass(int i, int classID) { _agents[i]->_class = classID; }
  float RVOSimulator::getAgentOrientation(int i) const { return _agents[i]->_o; }
  void RVOSimulator::setAgentOrientation(int i, float o) { _agents[i]->_o = o; }
  int RVOSimulator::getAgentGoal(int i) const { return _agents[i]->_goalID; }
  void RVOSimulator::setAgentGoal(int i, int goalID) { _agents[i]->_goalID = goalID; }
  float RVOSimulator::getAgentGoalRadius(int i) const { return _agents[i]->_gR; }
  void RVOSimulator::setAgentGoalRadius(int i, float gR) { _agents[i]->_gR = gR; }
  float RVOSimulator::getAgentPrefSpeed(int i) const { return _agents[i]->_prefSpeed; }
  void RVOSimulator::setAgentPrefSpeed(int i, float prefSpeed) { _agents[i]->_prefSpeed = prefSpeed; }
  float RVOSimulator::getAgentMaxSpeed(int i) const { return _agents[i]->_maxSpeed; }
  void RVOSimulator::setAgentMaxSpeed(int i, float maxSpeed) { _agents[i]->_maxSpeed = maxSpeed; }
  float RVOSimulator::getAgentMaxAccel(int i) const { return _agents[i]->_maxAccel; }
  void RVOSimulator::setAgentMaxAccel(int i, float maxAccel) { _agents[i]->_maxAccel = maxAccel; }
  float RVOSimulator::getAgentSafetyFactor(int i) const { return _agents[i]->_safetyFactor; }
  void RVOSimulator::setAgentSafetyFactor(int i, float safetyFactor) { _agents[i]->_safetyFactor = safetyFactor; }

  // Goal Getter/Setter 's
  int RVOSimulator::getNumGoals() const { return (int) _goals.size(); }
  const Vector2& RVOSimulator::getGoalPosition(int i) const { return _goals[i]->_vertex->_p; }
  int RVOSimulator::getGoalNumNeighbors(int i) const { return (int) _goals[i]->_vertex->_neighbors.size(); }
  int RVOSimulator::getGoalNeighbor(int i, int n) const { return _goals[i]->_vertex->_neighbors[n].second; }
  
  // Obstacle Getter/Setter 's
  int RVOSimulator::getNumObstacles() const { return (int) _obstacles.size(); }
  const Vector2& RVOSimulator::getObstaclePoint1(int i) const { return _obstacles[i]->_p1; }
  const Vector2& RVOSimulator::getObstaclePoint2(int i) const { return _obstacles[i]->_p2; }

  // Roadmap Getters/Setter 's
  int RVOSimulator::getNumRoadmapVertices() const { return (int) _roadmap->_vertices.size(); }
  const Vector2& RVOSimulator::getRoadmapVertexPosition(int i) const { return _roadmap->_vertices[i]->_p; }
  int RVOSimulator::getRoadmapVertexNumNeighbors(int i) const { return (int) _roadmap->_vertices[i]->_neighbors.size(); }
  int RVOSimulator::getRoadmapVertexNeighbor(int i, int n) const { return _roadmap->_vertices[i]->_neighbors[n].second; }

  // Agent adders
  void RVOSimulator::setAgentDefaults( int velSampleCountDefault, float neighborDistDefault, int maxNeighborsDefault, float rDefault, float gRDefault, float prefSpeedDefault, float maxSpeedDefault, float safetyFactorDefault, float maxAccelDefault, const Vector2& vDefault, float oDefault, int classDefault ) {
    _agentDefaultsHaveBeenSet = true;
    
    _defaultAgent->_velSampleCount = velSampleCountDefault;
    _defaultAgent->_neighborDist = neighborDistDefault;
    _defaultAgent->_maxNeighbors = maxNeighborsDefault;

    _defaultAgent->_class = classDefault;
    _defaultAgent->_r = rDefault;
    _defaultAgent->_v = vDefault;
    _defaultAgent->_maxAccel = maxAccelDefault;
    _defaultAgent->_gR = gRDefault;
    _defaultAgent->_prefSpeed = prefSpeedDefault;
    _defaultAgent->_maxSpeed = maxSpeedDefault;
    _defaultAgent->_o = oDefault;
    _defaultAgent->_safetyFactor = safetyFactorDefault;
  }
  
  int RVOSimulator::addAgent(const Vector2& startPosition, int goalID) {
    if (_simulationHasBeenInitialized) {
      std::cerr << "Simulation already initialized when adding agent." << std::endl;
      return RVO_SIMULATION_ALREADY_INITIALIZED_WHEN_ADDING_AGENT;
    }
    if ( ! _agentDefaultsHaveBeenSet ) {
      std::cerr << "Agent defaults have not been set when adding agent." << std::endl;
      return RVO_AGENT_DEFAULTS_HAVE_NOT_BEEN_SET_WHEN_ADDING_AGENT;
    }
    Agent* agent = new Agent(startPosition, goalID); 
    _agents.push_back(agent); 
    return (int) _agents.size() - 1; 
  }
  
  int RVOSimulator::addAgent(const Vector2& startPosition, int goalID, int velSampleCount, float neighborDist, int maxNeighbors, float r, float gR, float prefSpeed, float maxSpeed, float safetyFactor, float maxAccel, const Vector2& v, float o, int classID ) {
    if (_simulationHasBeenInitialized) {
      std::cerr << "Simulation already initialized when adding agent." << std::endl;
      return RVO_SIMULATION_ALREADY_INITIALIZED_WHEN_ADDING_AGENT;
    }
    Agent* agent = new Agent(startPosition, goalID, velSampleCount, neighborDist, maxNeighbors, classID, r, v, maxAccel, gR, prefSpeed, maxSpeed, o, safetyFactor); 
    _agents.push_back(agent); 
    return (int) _agents.size() - 1;
  }
  
  // Goal adder
  int RVOSimulator::addGoal(const Vector2& position) {
    if (_simulationHasBeenInitialized) {
      std::cerr << "Simulation already initialized when adding goal." << std::endl;
      return RVO_SIMULATION_ALREADY_INITIALIZED_WHEN_ADDING_GOAL;
    }
    Goal* goal = new Goal(position);
    _goals.push_back(goal);
    return (int) _goals.size() - 1;
  }

  // Obstacle Adder
  int RVOSimulator::addObstacle(const Vector2& point1, const Vector2& point2) { 
    if (_simulationHasBeenInitialized) {
      std::cerr << "Simulation already initialized when adding obstacle." << std::endl;
      return RVO_SIMULATION_ALREADY_INITIALIZED_WHEN_ADDING_OBSTACLE;
    }
    Obstacle* obstacle = new Obstacle(point1, point2); 
    _obstacles.push_back(obstacle); 
    return (int) _obstacles.size() - 1; 
  }

  // Roadmap Adders
  void RVOSimulator::setRoadmapAutomatic(bool automatic) { 
    _roadmap->_automatic = automatic;
  }

  int RVOSimulator::addRoadmapVertex(const Vector2& position) { 
    if (_simulationHasBeenInitialized) {
      std::cerr << "Simulation already initialized when adding roadmap vertex." << std::endl;
      return RVO_SIMULATION_ALREADY_INITIALIZED_WHEN_ADDING_ROADMAP_VERTEX;
    }
    _roadmap->addVertex(position); 
    return (int) _roadmap->_vertices.size() - 1; 
  }
  
  int RVOSimulator::addRoadmapEdge(int vertexID1, int vertexID2) { 
    if (_simulationHasBeenInitialized) {
      std::cerr << "Simulation already initialized when adding roadmap edge." << std::endl;
      return RVO_SIMULATION_ALREADY_INITIALIZED_WHEN_ADDING_ROADMAP_EDGE;
    }
    _roadmap->addEdge(vertexID1, vertexID2); 
    return RVO_SUCCESS; 
  }

  // Initialize Simulation
  void RVOSimulator::initSimulation(void) {
    _simulationHasBeenInitialized = true;
    
    _roadmap->init();
    
    _kdTree = new KDTree();
    _kdTree->buildObstacleTree();

    #pragma omp parallel for
    for (int i = 0; i < (int) _goals.size(); ++i) {
      _goals[i]->computeShortestPathTree();
    }
  }
}