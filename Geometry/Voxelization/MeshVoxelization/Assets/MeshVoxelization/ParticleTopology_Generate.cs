using bluebean;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static bluebean.ParticleTopology;

namespace bluebean
{
    public partial class ParticleTopology
    {
        public class SimplexComparer : IEqualityComparer<Vector3Int>
        {
            public bool Equals(Vector3Int a, Vector3Int b)
            {
                return (a.x == b.x || a.x == b.y || a.x == b.z) &&
                       (a.y == b.x || a.y == b.y || a.y == b.z) &&
                       (a.z == b.x || a.z == b.y || a.z == b.z);

            }

            public int GetHashCode(Vector3Int item)
            {
                return item.GetHashCode();

            }
        }

        [Flags]
        public enum ParticleType
        {
            None = 0,
            Bone = 1 << 0,
            Volume = 1 << 1,
            Surface = 1 << 2,
            All = Bone | Volume | Surface
        }

        public enum SurfaceSamplingMode
        {
            None,
            Vertices,
            Voxels
        }

        public enum VolumeSamplingMode
        {
            None,
            Voxels
        }

        [Flags]
        public enum VoxelConnectivity
        {
            None = 0,
            Faces = 1 << 0,
            Edges = 1 << 1,
            Vertices = 1 << 2,
            All = Faces | Edges | Vertices
        }

        public const float DEFAULT_PARTICLE_MASS = 0.1f;
        private Matrix4x4 blueprintTransform;

        public Mesh inputMesh;               /**< Mesh used to generate the Particles*/
        public Vector3 scale = Vector3.one;
        public Quaternion rotation = Quaternion.identity;

        [Tooltip("Method used to distribute particles on the surface of the mesh.")]
        public SurfaceSamplingMode surfaceSamplingMode = SurfaceSamplingMode.Voxels;

        [Tooltip("Resolution of the surface particle distribution.")]
        [Range(2, 128)]
        public int surfaceResolution = 16;

        [Tooltip("Method used to distribute particles on the volume of the mesh.")]
        public VolumeSamplingMode volumeSamplingMode = VolumeSamplingMode.None;

        [Tooltip("Resolution of the volume particle distribution.")]
        [Range(2, 128)]
        public int volumeResolution = 16;

        [Range(0, 1)]
        [Tooltip("Amount of smoothing applied to particle positions.")]
        public float smoothing = 0.25f;

        [Tooltip("Voxel resolution used to analyze the shape of the mesh.")]
        [Range(2, 128)]
        public int shapeResolution = 48;

        public UnityEvent<IParticleCollection> EventOnGenerateComplete;
        //public event Action EventOnGenerateComplete;

        private VoxelDistanceField m_DistanceField;
        private VoxelPathFinder m_PathFinder;
        private List<int>[] voxelToParticles;

        [HideInInspector] public Mesh generatedMesh = null;
        [HideInInspector] public int[] vertexToParticle = null;
        [HideInInspector] public List<ParticleType> particleType = null;

        private GraphColoring colorizer;
        public MeshVoxelizer surfaceVoxelizer { get; private set; }
        public MeshVoxelizer volumeVoxelizer { get; private set; }
        public MeshVoxelizer shapeVoxelizer { get; private set; }

        public IEnumerator Generate()
        {
            if (inputMesh == null || !inputMesh.isReadable)
            {
                Debug.LogError("The input mesh is null, or not readable.");
                yield break;
            }

            // Prepare candidate particle arrays: 
            List<Vector3> particlePositions = new List<Vector3>();
            List<Vector3> particleNormals = new List<Vector3>();

            // Transform mesh data:
            blueprintTransform = Matrix4x4.TRS(Vector3.zero, rotation, scale);
            Vector3[] vertices = inputMesh.vertices;
            Vector3[] normals = inputMesh.normals;
            int[] tris = inputMesh.triangles;
            Bounds transformedBounds = new Bounds();

            for (int i = 0; i < vertices.Length; ++i)
            {
                vertices[i] = blueprintTransform.MultiplyPoint3x4(vertices[i]);
                transformedBounds.Encapsulate(vertices[i]);
            }

            for (int i = 0; i < normals.Length; ++i)
                normals[i] = Vector3.Normalize(blueprintTransform.MultiplyVector(normals[i]));

            // initialize arrays:
            particleType = new List<ParticleType>();
            // initialize graph coloring:
            colorizer = new GraphColoring();

            //voxelize for cluster placement:
            var voxelize = VoxelizeForShapeAnalysis(transformedBounds.size);
            while (voxelize.MoveNext()) yield return voxelize.Current;
            //voxelize for surface:
            voxelize = VoxelizeForSurfaceSampling(transformedBounds.size);
            while (voxelize.MoveNext()) yield return voxelize.Current;

            if (surfaceSamplingMode == SurfaceSamplingMode.Vertices)
            {
                var sv = VertexSampling(vertices, particlePositions);
                while (sv.MoveNext()) yield return sv.Current;

                var mp = MapVerticesToParticles(vertices, normals, particlePositions, particleNormals);
                while (mp.MoveNext()) yield return mp.Current;

                var ss = SurfaceMeshShapeMatchingConstraints(particlePositions, tris);
                while (ss.MoveNext()) yield return ss.Current;
            }
            if (volumeSamplingMode == VolumeSamplingMode.Voxels)
            {
                voxelize = VoxelizeForVolumeSampling(transformedBounds.size);
                while (voxelize.MoveNext()) yield return voxelize.Current;

                var voxelType = surfaceSamplingMode != SurfaceSamplingMode.None ? MeshVoxelizer.Voxel.Inside : MeshVoxelizer.Voxel.Inside | MeshVoxelizer.Voxel.Boundary;
                var volume = VoxelSampling(volumeVoxelizer, particlePositions, voxelType, ParticleType.Volume);
                while (volume.MoveNext()) yield return volume.Current;

                var ip = InsertParticlesIntoVoxels(volumeVoxelizer, particlePositions);
                while (ip.MoveNext()) yield return ip.Current;

                var vc = CreateClustersFromVoxels(volumeVoxelizer, particlePositions, VoxelConnectivity.Faces, new List<ParticleType>() { ParticleType.Volume });
                while (vc.MoveNext()) yield return vc.Current;

                vc = CreateClustersFromVoxels(volumeVoxelizer, particlePositions, VoxelConnectivity.All, new List<ParticleType>() { ParticleType.Volume | ParticleType.Surface });
                while (vc.MoveNext()) yield return vc.Current;

                var mp = MapVerticesToParticles(vertices, normals, particlePositions, particleNormals);
                while (mp.MoveNext()) yield return mp.Current;
            }

            // generate particles:
            var generate = GenerateParticles(particlePositions, particleNormals);
            while (generate.MoveNext()) yield return generate.Current;

            // generate shape matching constraints:
            IEnumerator bc = CreateShapeMatchingConstraints(particlePositions);
            while (bc.MoveNext()) yield return bc.Current;

            // generate simplices:
            IEnumerator s = CreateSimplices(particlePositions, tris);
            while (s.MoveNext()) yield return s.Current;

            generatedMesh = inputMesh;

            if (EventOnGenerateComplete != null)
            {
                EventOnGenerateComplete.Invoke(this);
            }
        }

        protected virtual IEnumerator CreateSimplices(List<Vector3> particles, int[] meshTriangles)
        {
            HashSet<Vector3Int> simplices = new HashSet<Vector3Int>(new SimplexComparer());

            // Generate deformable triangles:
            int i;
            for (i = 0; i < meshTriangles.Length; i += 3)
            {
                int p1 = vertexToParticle[meshTriangles[i]];
                int p2 = vertexToParticle[meshTriangles[i + 1]];
                int p3 = vertexToParticle[meshTriangles[i + 2]];

                simplices.Add(new Vector3Int(p1, p2, p3));

                if (i % 500 == 0)
                    yield return new CoroutineJob.ProgressInfo("ParticleTopology: generating simplices geometry...", i / (float)meshTriangles.Length);
            }

            i = 0;

            this.triangles = new int[simplices.Count * 3];
            foreach (Vector3Int s in simplices)
            {
                triangles[i++] = s.x;
                triangles[i++] = s.y;
                triangles[i++] = s.z;
            }
        }

        protected virtual IEnumerator CreateShapeMatchingConstraints(List<Vector3> particles)
        {
            //Create shape matching clusters:
            //shapeMatchingConstraintsData = new ObiShapeMatchingConstraintsData();

            List<int> constraintColors = new List<int>();
            var colorize = colorizer.Colorize("ObiSoftbody: coloring shape matching constraints...", constraintColors);
            while (colorize.MoveNext())
                yield return colorize.Current;

            var particleIndices = colorizer.particleIndices;
            var constraintIndices = colorizer.constraintIndices;

            for (int i = 0; i < constraintColors.Count; ++i)
            {
                int color = constraintColors[i];
                int cIndex = constraintIndices[i];

                // Add a new batch if needed:
                //if (color >= shapeMatchingConstraintsData.GetBatchCount())
                //    shapeMatchingConstraintsData.AddBatch(new ObiShapeMatchingConstraintsBatch());

                int amount = constraintIndices[i + 1] - cIndex;
                int[] clusterIndices = new int[amount];
                for (int j = 0; j < amount; ++j)
                    clusterIndices[j] = particleIndices[cIndex + j];

                //shapeMatchingConstraintsData.batches[color].AddConstraint(clusterIndices, false);
            }

            // Set initial amount of active constraints:
            //for (int i = 0; i < shapeMatchingConstraintsData.batches.Count; ++i)
            //{
            //    shapeMatchingConstraintsData.batches[i].activeConstraintCount = shapeMatchingConstraintsData.batches[i].constraintCount;
            //}

            yield return new CoroutineJob.ProgressInfo("ParticleTopology: batching constraints", 1);
        }

        private IEnumerator GenerateParticles(List<Vector3> particlePositions, List<Vector3> particleNormals)
        {
            float particleRadius = PhysicsUtil.sqrt3 * 0.5f * surfaceVoxelizer.voxelSize;

            positions = new Vector3[particlePositions.Count];
            //orientations = new Quaternion[particlePositions.Count];
            //restPositions = new Vector4[particlePositions.Count];
            //restOrientations = new Quaternion[particlePositions.Count];
            //velocities = new Vector3[particlePositions.Count];
            //angularVelocities = new Vector3[particlePositions.Count];
            invMasses = new float[particlePositions.Count];
            //invRotationalMasses = new float[particlePositions.Count];
            //principalRadii = new Vector3[particlePositions.Count];
            radius = new float[particlePositions.Count];
            //filters = new int[particlePositions.Count];
            colors = new Color[particlePositions.Count];

            //m_ActiveParticleCount = particlePositions.Count;

            for (int i = 0; i < particlePositions.Count; ++i)
            {
                // Perform ellipsoid fitting:
                List<Vector3> neighborhood = new List<Vector3>();

                Vector3 centroid = particlePositions[i];
                Quaternion orientation = Quaternion.identity;
                Vector3 principalValues = Vector3.one * particleRadius;

                // Calculate high-def voxel neighborhood extents:
                var anisotropyNeighborhood = Vector3.one * shapeVoxelizer.voxelSize * PhysicsUtil.sqrt3 * 2;
                Vector3Int min = shapeVoxelizer.GetPointVoxel(centroid - anisotropyNeighborhood) - shapeVoxelizer.Origin;
                Vector3Int max = shapeVoxelizer.GetPointVoxel(centroid + anisotropyNeighborhood) - shapeVoxelizer.Origin;

                for (int nx = min.x; nx <= max.x; ++nx)
                    for (int ny = min.y; ny <= max.y; ++ny)
                        for (int nz = min.z; nz <= max.z; ++nz)
                        {
                            if (shapeVoxelizer.VoxelExists(nx, ny, nz) &&
                                shapeVoxelizer[nx, ny, nz] != MeshVoxelizer.Voxel.Outside)
                            {
                                Vector3 voxelCenter = shapeVoxelizer.GetVoxelCenter(new Vector3Int(nx, ny, nz));
                                neighborhood.Add(voxelCenter);
                            }
                        }

                // distance field normal:
                Vector3 dfnormal = m_DistanceField.SampleFiltered(centroid.x, centroid.y, centroid.z);

                // if the distance field normal isn't robust enough, use average vertex normal:
                if (Vector3.Dot(dfnormal, particleNormals[i]) < 0.75f)
                    dfnormal = particleNormals[i];

                // if the particle has a non-empty neighborhood, perform ellipsoidal fitting:
                //if (neighborhood.Count > 0)
                //    ObiUtils.GetPointCloudAnisotropy(neighborhood, maxAnisotropy, particleRadius, dfnormal, ref centroid, ref orientation, ref principalValues);

                //invRotationalMasses[i] = invMasses[i] = 1.0f;
                positions[i] = Vector3.Lerp(particlePositions[i], centroid, smoothing);
                //restPositions[i] = positions[i];
                //restPositions[i][3] = 1; // activate rest position.
                //orientations[i] = orientation;
                //restOrientations[i] = orientation;
                //principalRadii[i] = principalValues;
                radius[i] = particleRadius;
                //filters[i] = ObiUtils.MakeFilter(ObiUtils.CollideWithEverything, 1);
                colors[i] = Color.white;

                if (i % 100 == 0)
                    yield return new CoroutineJob.ProgressInfo("ParticleTopology: generating particles...", i / (float)particlePositions.Count);
            }
        }

        protected void ConnectToNeighborParticles(MeshVoxelizer voxelizer, int particle, List<Vector3> particles, List<ParticleType> allowed, int x, int y, int z, Vector3Int[] neighborhood, float clusterSize, List<int> cluster)
        {
            Vector3Int startVoxel = shapeVoxelizer.GetPointVoxel(particles[particle]) - shapeVoxelizer.Origin;
            startVoxel = m_PathFinder.FindClosestNonEmptyVoxel(startVoxel).coordinates;

            for (int j = 0; j < neighborhood.Length; ++j)
            {
                var voxel = neighborhood[j];
                int index = voxelizer.GetVoxelIndex(x + voxel.x, y + voxel.y, z + voxel.z);

                foreach (var neighbor in voxelToParticles[index])
                {
                    if (!allowed.Contains(particleType[particle] | particleType[neighbor]))
                        continue;

                    Vector3Int endVoxel = shapeVoxelizer.GetPointVoxel(particles[neighbor]) - shapeVoxelizer.Origin;
                    endVoxel = m_PathFinder.FindClosestNonEmptyVoxel(endVoxel).coordinates;

                    var path = m_PathFinder.FindPath(startVoxel, endVoxel);
                    float geodesicDistance = path.distance;

                    if (geodesicDistance <= clusterSize)
                        cluster.Add(neighbor);
                }
            }
        }

        protected IEnumerator CreateClustersFromVoxels(MeshVoxelizer voxelizer, List<Vector3> particles, VoxelConnectivity connectivity, List<ParticleType> allowed)
        {
            float clusterSize = PhysicsUtil.sqrt3 * voxelizer.voxelSize * 1.5f;

            List<int> cluster = new List<int>();
            for (int i = 0; i < particles.Count; ++i)
            {
                Vector3Int voxel = voxelizer.GetPointVoxel(particles[i]) - voxelizer.Origin;

                cluster.Clear();
                cluster.Add(i);

                if ((connectivity & VoxelConnectivity.Faces) != 0)
                    ConnectToNeighborParticles(voxelizer, i, particles, allowed, voxel.x, voxel.y, voxel.z, MeshVoxelizer.faceNeighborhood, clusterSize, cluster);

                if ((connectivity & VoxelConnectivity.Edges) != 0)
                    ConnectToNeighborParticles(voxelizer, i, particles, allowed, voxel.x, voxel.y, voxel.z, MeshVoxelizer.edgeNeighborhood, clusterSize, cluster);

                if ((connectivity & VoxelConnectivity.Vertices) != 0)
                    ConnectToNeighborParticles(voxelizer, i, particles, allowed, voxel.x, voxel.y, voxel.z, MeshVoxelizer.vertexNeighborhood, clusterSize, cluster);

                colorizer.AddConstraint(cluster.ToArray());

                if (i % 100 == 0)
                    yield return new CoroutineJob.ProgressInfo("ParticleTopology: generating shape matching clusters...", i / (float)voxelizer.voxelCount);
            }
        }

        private IEnumerator InsertParticlesIntoVoxels(MeshVoxelizer voxelizer, List<Vector3> particles)
        {
            voxelToParticles = new List<int>[voxelizer.voxelCount];
            for (int i = 0; i < voxelToParticles.Length; ++i)
                voxelToParticles[i] = new List<int>(4);

            for (int i = 0; i < particles.Count; ++i)
            {
                Vector3Int voxel = voxelizer.GetPointVoxel(particles[i]) - voxelizer.Origin;
                int index = voxelizer.GetVoxelIndex(voxel.x, voxel.y, voxel.z);

                voxelToParticles[index].Add(i);

                if (i % 100 == 0)
                    yield return new CoroutineJob.ProgressInfo("ParticleTopology: inserting particles into voxels...", i / (float)particles.Count);
            }
        }

        private IEnumerator VoxelSampling(MeshVoxelizer voxelizer, List<Vector3> particles, MeshVoxelizer.Voxel voxelType, ParticleType pType)
        {
            int i = 0;

            for (int x = 0; x < voxelizer.resolution.x; ++x)
                for (int y = 0; y < voxelizer.resolution.y; ++y)
                    for (int z = 0; z < voxelizer.resolution.z; ++z)
                    {
                        if ((voxelizer[x, y, z] & voxelType) != 0)
                        {
                            var voxel = new Vector3Int(x, y, z);
                            Vector3 voxelCenter = voxelizer.GetVoxelCenter(voxel);

                            particles.Add(voxelCenter);
                            particleType.Add(pType);
                        }
                        if (++i % 1000 == 0)
                            yield return new CoroutineJob.ProgressInfo("ParticleTopology: sampling voxels...", i / (float)voxelizer.voxelCount);
                    }
        }

        private IEnumerator VoxelizeForVolumeSampling(Vector3 boundsSize)
        {
            float longestSide = Mathf.Max(Mathf.Max(boundsSize.x, boundsSize.y), boundsSize.z);
            float size = longestSide / volumeResolution;

            volumeVoxelizer = new MeshVoxelizer(inputMesh, size);
            var voxelizeCoroutine = volumeVoxelizer.Voxelize(blueprintTransform, true);
            while (voxelizeCoroutine.MoveNext())
                yield return voxelizeCoroutine.Current;

            volumeVoxelizer.BoundaryThinning();
        }

        private IEnumerator VoxelizeForShapeAnalysis(Vector3 boundsSize)
        {
            // Calculate voxel size: 
            float longestSide = Mathf.Max(Mathf.Max(boundsSize.x, boundsSize.y), boundsSize.z);
            float size = longestSide / shapeResolution;

            // Voxelize mesh and calculate discrete distance field:
            shapeVoxelizer = new MeshVoxelizer(inputMesh, size);
            var voxelizeCoroutine = shapeVoxelizer.Voxelize(blueprintTransform);
            while (voxelizeCoroutine.MoveNext())
                yield return voxelizeCoroutine.Current;

            shapeVoxelizer.BoundaryThinning();

            // Generate distance field:
            m_DistanceField = new VoxelDistanceField(shapeVoxelizer);
            var dfCoroutine = m_DistanceField.JumpFlood();
            while (dfCoroutine.MoveNext())
                yield return dfCoroutine.Current;

            // Create path finder:
            m_PathFinder = new VoxelPathFinder(shapeVoxelizer);
        }

        private IEnumerator VoxelizeForSurfaceSampling(Vector3 boundsSize)
        {
            float longestSide = Mathf.Max(Mathf.Max(boundsSize.x, boundsSize.y), boundsSize.z);
            float size = longestSide / surfaceResolution;

            surfaceVoxelizer = new MeshVoxelizer(inputMesh, size);
            var voxelizeCoroutine = surfaceVoxelizer.Voxelize(blueprintTransform, true);
            while (voxelizeCoroutine.MoveNext())
                yield return voxelizeCoroutine.Current;

            surfaceVoxelizer.BoundaryThinning();
        }

        private IEnumerator VertexSampling(Vector3[] vertices, List<Vector3> particlePositions)
        {
            float particleRadius = PhysicsUtil.sqrt3 * 0.5f * surfaceVoxelizer.voxelSize;

            for (int i = 0; i < vertices.Length; ++i)
            {
                bool valid = true;
                for (int j = 0; j < particlePositions.Count; ++j)
                {
                    if (Vector3.Distance(vertices[i], particlePositions[j]) < particleRadius * 1.2f)
                    {
                        valid = false;
                        break;
                    }
                }

                if (valid)
                {
                    particlePositions.Add(vertices[i]);
                    particleType.Add(ParticleType.Surface);
                }

                if (i % 100 == 0)
                    yield return new CoroutineJob.ProgressInfo("ParticleTopology: sampling surface...", i / (float)vertices.Length);
            }
        }

        private IEnumerator MapVerticesToParticles(Vector3[] vertices, Vector3[] normals, List<Vector3> particlePositions, List<Vector3> particleNormals)
        {
            vertexToParticle = new int[vertices.Length];

            for (int i = 0; i < particlePositions.Count; ++i)
                particleNormals.Add(Vector3.zero);

            // Find out the closest particle to each vertex:
            for (int i = 0; i < vertices.Length; ++i)
            {
                float minDistance = float.MaxValue;
                for (int j = 0; j < particlePositions.Count; ++j)
                {
                    float distance = Vector3.SqrMagnitude(vertices[i] - particlePositions[j]);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        vertexToParticle[i] = j;
                    }
                }

                particleNormals[vertexToParticle[i]] += normals[i];

                if (i % 100 == 0)
                    yield return new CoroutineJob.ProgressInfo("ParticleTopology: mapping vertices to particles...", i / (float)vertices.Length);
            }

            for (int i = 0; i < particleNormals.Count; ++i)
                particleNormals[i] = Vector3.Normalize(particleNormals[i]);
        }

        protected IEnumerator SurfaceMeshShapeMatchingConstraints(List<Vector3> particles, int[] meshTriangles)
        {
            HashSet<int>[] connections = new HashSet<int>[particles.Count];
            for (int i = 0; i < connections.Length; ++i)
                connections[i] = new HashSet<int>();

            for (int i = 0; i < meshTriangles.Length; i += 3)
            {
                int p1 = vertexToParticle[meshTriangles[i]];
                int p2 = vertexToParticle[meshTriangles[i + 1]];
                int p3 = vertexToParticle[meshTriangles[i + 2]];

                if (p1 != p2)
                {
                    connections[p1].Add(p2);
                    connections[p2].Add(p1);
                }

                if (p1 != p3)
                {
                    connections[p1].Add(p3);
                    connections[p3].Add(p1);
                }

                if (p2 != p3)
                {
                    connections[p2].Add(p3);
                    connections[p3].Add(p2);
                }

                if (i % 100 == 0)
                    yield return new CoroutineJob.ProgressInfo("ParticleTopology: generating shape matching clusters...", i / (float)meshTriangles.Length);
            }

            List<int> cluster = new List<int>();
            for (int i = 0; i < connections.Length; ++i)
            {
                cluster.Clear();

                cluster.Add(i);
                foreach (var n in connections[i])
                    cluster.Add(n);

                colorizer.AddConstraint(cluster.ToArray());
            }
        }
    }
}
