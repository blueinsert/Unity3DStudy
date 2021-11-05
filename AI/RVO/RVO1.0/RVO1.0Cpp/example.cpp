#ifndef _WIN32_WINNT    // Allow use of features specific to Windows XP or later.                   
#define _WIN32_WINNT 0x0501  // Change this to the appropriate value to target other versions of Windows.
#endif            

#include <stdio.h>
#include <tchar.h>
#include <stdlib.h>

#include "RVOSimulator.h"

void setupScenario( RVO::RVOSimulator * sim ) {
  // Specify global time step of the simulation
  sim->setTimeStep( 0.25f );

  // Specify default parameters for agents that are subsequently added
  sim->setAgentDefaults( 250, 15.0f, 10, 2.0f, 3.0f, 1.0f, 2.0f, 7.5f, 1.0f );

  // Add agents (and simulataneously their goals), specifying their start position and goal ID
  sim->addAgent( RVO::Vector2(-50.0f, -50.0f), sim->addGoal( RVO::Vector2(50.0f, 50.0f) ) );
  sim->addAgent( RVO::Vector2(50.0f, -50.0f), sim->addGoal( RVO::Vector2(-50.0f, 50.0f) ) );
  sim->addAgent( RVO::Vector2(50.0f, 50.0f), sim->addGoal( RVO::Vector2(-50.0f, -50.0f) ) );
  sim->addAgent( RVO::Vector2(-50.0f, 50.0f), sim->addGoal( RVO::Vector2(50.0f, -50.0f) ) );

  // Add (line segment) obstacles, specifying both endpoints of the line segments
  //顺时针
  sim->addObstacle( RVO::Vector2(-7.0f, -20.0f), RVO::Vector2(-7.0f, 20.0f) );
  sim->addObstacle( RVO::Vector2(-7.0f, 20.0f), RVO::Vector2(7.0f, 20.0f) );
  sim->addObstacle( RVO::Vector2(7.0f, 20.0f), RVO::Vector2(7.0f, -20.0f) );
  sim->addObstacle( RVO::Vector2(7.0f, -20.0f), RVO::Vector2(-7.0f, -20.0f) );

  // Add roadmap vertices, specifying their position
  sim->addRoadmapVertex( RVO::Vector2(-10.0f, -23.0f) );
  sim->addRoadmapVertex( RVO::Vector2(-10.0f, 23.0f) );
  sim->addRoadmapVertex( RVO::Vector2(10.0f, 23.0f) );
  sim->addRoadmapVertex( RVO::Vector2(10.0f, -23.0f) );

  // Do not automatically create edges between mutually visible roadmap vertices
  sim->setRoadmapAutomatic( false );

  // Manually specify edges between vertices, specifying the ID's of the vertices the edges connect
  sim->addRoadmapEdge( 0, 1 );
  sim->addRoadmapEdge( 1, 2 );
  sim->addRoadmapEdge( 2, 3 );
  sim->addRoadmapEdge( 3, 0 );
}


void updateVisualization( RVO::RVOSimulator * sim ) {
  // Output the current global time
  std::cout << sim->getGlobalTime() << " ";

  // Output the position and orientation for all the agents
  for (int i = 0; i < sim->getNumAgents(); ++i) {
    std::cout << sim->getAgentPosition( i ) << " " << sim->getAgentOrientation( i ) << " ";
  }

  std::cout << std::endl;
}


int _tmain(int argc, _TCHAR* argv[])
{
  // Create a simulator instance 
  RVO::RVOSimulator * sim = RVO::RVOSimulator::Instance();

  // Set up the scenario
  setupScenario( sim );       

  // Initialize the simulation
  sim->initSimulation();

  // Perform (and manipulate) the simulation
  do {                       
    updateVisualization( sim );
    sim->doStep();
  } while ( !sim->getReachedGoal() );

  delete sim;

  return 0;
}

