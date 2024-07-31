using bluebean;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace bluebean
{
#if UNITY_EDITOR
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
        private Matrix4x4 m_localTransform;

        public Mesh m_inputMesh;               /**< Mesh used to generate the Particles*/
        public Vector3 m_scale = Vector3.one;
        public Quaternion m_rotation = Quaternion.identity;

        [Tooltip("Method used to distribute particles on the surface of the mesh.")]
        [Header("表面采样模式")]
        public SurfaceSamplingMode m_surfaceSamplingMode = SurfaceSamplingMode.Voxels;

        [Tooltip("Resolution of the surface particle distribution.")]
        [Range(2, 128)]
        [Header("表面采样解析度")]
        public int m_surfaceResolution = 16;

        [Tooltip("Method used to distribute particles on the volume of the mesh.")]
        [Header("体采样模式")]
        public VolumeSamplingMode m_volumeSamplingMode = VolumeSamplingMode.None;

        [Tooltip("Resolution of the volume particle distribution.")]
        [Range(2, 128)]
        [Header("表面采样解析度")]
        public int m_volumeResolution = 16;

        [Range(0, 1)]
        [Tooltip("Amount of smoothing applied to particle positions.")]
        public float m_smoothing = 0.25f;

        [Tooltip("Voxel resolution used to analyze the shape of the mesh.")]
        [Range(2, 128)]
        public int m_shapeResolution = 48;

        public UnityEvent<IParticleCollection> EventOnGenerateComplete;
        //public event Action EventOnGenerateComplete;

        private VoxelDistanceField m_distanceField;
        private VoxelPathFinder m_pathFinder;
        private List<int>[] m_voxelToParticles;

        [HideInInspector] public Mesh m_generatedMesh = null;
        /// <summary>
        /// 顶点索引到粒子索引的映射
        /// </summary>
        [HideInInspector] public int[] m_vertexToParticle = null;
        [HideInInspector] public List<ParticleType> m_particleTypes = null;

        private GraphColoring m_colorizer;
        //下面三个区别是对应不同解析度，另外voxelDistanceField和pathfinder用的ShapeVoxelizer
        public MeshVoxelizer SurfaceVoxelizer { get; private set; }
        public MeshVoxelizer VolumeVoxelizer { get; private set; }
        public MeshVoxelizer ShapeVoxelizer { get; private set; }

        public IEnumerator Generate()
        {
            if (m_inputMesh == null || !m_inputMesh.isReadable)
            {
                Debug.LogError("The input mesh is null, or not readable.");
                yield break;
            }

            // Prepare candidate particle arrays: 
            List<Vector3> particlePositions = new List<Vector3>();
            List<Vector3> particleNormals = new List<Vector3>();

            // Transform mesh data:
            m_localTransform = Matrix4x4.TRS(Vector3.zero, m_rotation, m_scale);
            Vector3[] vertices = m_inputMesh.vertices;
            Vector3[] normals = m_inputMesh.normals;
            int[] tris = m_inputMesh.triangles;
            Bounds transformedBounds = new Bounds();

            for (int i = 0; i < vertices.Length; ++i)
            {
                vertices[i] = m_localTransform.MultiplyPoint3x4(vertices[i]);
                transformedBounds.Encapsulate(vertices[i]);
            }

            for (int i = 0; i < normals.Length; ++i)
                normals[i] = Vector3.Normalize(m_localTransform.MultiplyVector(normals[i]));

            // initialize arrays:
            m_particleTypes = new List<ParticleType>();
            // initialize graph coloring:
            m_colorizer = new GraphColoring();

            //voxelize for cluster placement:
            var voxelize = VoxelizeForShapeAnalysis(transformedBounds.size);
            while (voxelize.MoveNext()) yield return voxelize.Current;
            //voxelize for surface:
            voxelize = VoxelizeForSurfaceSampling(transformedBounds.size);
            while (voxelize.MoveNext()) yield return voxelize.Current;

            if (m_surfaceSamplingMode == SurfaceSamplingMode.Vertices)
            {
                var sv = VertexSampling(vertices, particlePositions);
                while (sv.MoveNext()) yield return sv.Current;

                var mp = MapVerticesToParticles(vertices, normals, particlePositions, particleNormals);
                while (mp.MoveNext()) yield return mp.Current;

                var ss = SurfaceMeshShapeMatchingConstraints(particlePositions, tris);
                while (ss.MoveNext()) yield return ss.Current;
            }

            if (m_surfaceSamplingMode == SurfaceSamplingMode.Voxels)
            {
                var surface = VoxelSampling(SurfaceVoxelizer, particlePositions, MeshVoxelizer.Voxel.Boundary, ParticleType.Surface);
                while (surface.MoveNext()) yield return surface.Current;
                //因为之前VoxelSampling一个体素只会产生一个粒子，
                //这里似乎缺乏意义
                IEnumerator ip = InsertParticlesIntoVoxels(SurfaceVoxelizer, particlePositions);
                while (ip.MoveNext()) yield return ip.Current;

                IEnumerator vc = CreateClustersFromVoxels(SurfaceVoxelizer, particlePositions, VoxelConnectivity.Faces | VoxelConnectivity.Edges, new List<ParticleType>() { ParticleType.Surface });
                while (vc.MoveNext()) yield return vc.Current;

                for (int i = 0; i < particlePositions.Count; ++i)
                    particlePositions[i] = ProjectOnMesh(particlePositions[i], vertices, tris);

                var mp = MapVerticesToParticles(vertices, normals, particlePositions, particleNormals);
                while (mp.MoveNext()) yield return mp.Current;

            }

            if (m_volumeSamplingMode == VolumeSamplingMode.Voxels)
            {
                voxelize = VoxelizeForVolumeSampling(transformedBounds.size);
                while (voxelize.MoveNext()) yield return voxelize.Current;

                var voxelType = m_surfaceSamplingMode != SurfaceSamplingMode.None ? MeshVoxelizer.Voxel.Inside : MeshVoxelizer.Voxel.Inside | MeshVoxelizer.Voxel.Boundary;
                var volume = VoxelSampling(VolumeVoxelizer, particlePositions, voxelType, ParticleType.Volume);
                while (volume.MoveNext()) yield return volume.Current;

                var ip = InsertParticlesIntoVoxels(VolumeVoxelizer, particlePositions);
                while (ip.MoveNext()) yield return ip.Current;

                var vc = CreateClustersFromVoxels(VolumeVoxelizer, particlePositions, VoxelConnectivity.Faces, new List<ParticleType>() { ParticleType.Volume });
                while (vc.MoveNext()) yield return vc.Current;

                vc = CreateClustersFromVoxels(VolumeVoxelizer, particlePositions, VoxelConnectivity.All, new List<ParticleType>() { ParticleType.Volume | ParticleType.Surface });
                while (vc.MoveNext()) yield return vc.Current;

                var mp = MapVerticesToParticles(vertices, normals, particlePositions, particleNormals);
                while (mp.MoveNext()) yield return mp.Current;
            }

            // generate particles:
            var generate = GenerateParticles(particlePositions, particleNormals);
            while (generate.MoveNext()) yield return generate.Current;

            // generate shape matching constraints:
            //IEnumerator bc = CreateShapeMatchingConstraints(particlePositions);
            //while (bc.MoveNext()) yield return bc.Current;

            // generate simplices:
            IEnumerator s = CreateSimplices(particlePositions, tris);
            while (s.MoveNext()) yield return s.Current;

            m_generatedMesh = m_inputMesh;

            if (EventOnGenerateComplete != null)
            {
                EventOnGenerateComplete.Invoke(this);
            }
        }

        private Vector3 ProjectOnMesh(Vector3 point, Vector3[] vertices, int[] tris)
        {
            Vector3 triProjection;
            Vector3 meshProjection = point;
            float min = float.MaxValue;

            var voxel = SurfaceVoxelizer.GetPointVoxel(point) - SurfaceVoxelizer.Origin;
            var triangleIndices = SurfaceVoxelizer.GetTrianglesOverlappingVoxel(SurfaceVoxelizer.GetVoxelIndex(voxel.x, voxel.y, voxel.z));

            if (triangleIndices != null)
            {
                foreach (int i in triangleIndices)
                {
                    PhysicsUtil.NearestPointOnTri(vertices[tris[i * 3]], vertices[tris[i * 3 + 1]], vertices[tris[i * 3 + 2]], point, out triProjection);
                    float dist = Vector3.SqrMagnitude(triProjection - point);
                    if (dist < min)
                    {
                        min = dist;
                        meshProjection = triProjection;
                    }
                }
            }
            return meshProjection;
        }

        protected virtual IEnumerator CreateSimplices(List<Vector3> particles, int[] meshTriangles)
        {
            HashSet<Vector3Int> simplices = new HashSet<Vector3Int>(new SimplexComparer());

            // Generate deformable triangles:
            int i;
            for (i = 0; i < meshTriangles.Length; i += 3)
            {
                int p1 = m_vertexToParticle[meshTriangles[i]];
                int p2 = m_vertexToParticle[meshTriangles[i + 1]];
                int p3 = m_vertexToParticle[meshTriangles[i + 2]];

                simplices.Add(new Vector3Int(p1, p2, p3));

                if (i % 500 == 0)
                    yield return new CoroutineJob.ProgressInfo("ParticleTopology: generating simplices geometry...", i / (float)meshTriangles.Length);
            }

            i = 0;

            this.m_triangles = new int[simplices.Count * 3];
            foreach (Vector3Int s in simplices)
            {
                m_triangles[i++] = s.x;
                m_triangles[i++] = s.y;
                m_triangles[i++] = s.z;
            }
        }

        protected virtual IEnumerator CreateShapeMatchingConstraints(List<Vector3> particles)
        {
            //Create shape matching clusters:
            //shapeMatchingConstraintsData = new ObiShapeMatchingConstraintsData();

            List<int> constraintColors = new List<int>();
            //var colorize = m_colorizer.Colorize("coloring shape matching constraints...", constraintColors);
            //while (colorize.MoveNext())
            //    yield return colorize.Current;

            var particleIndices = m_colorizer.particleIndices;
            var constraintIndices = m_colorizer.constraintIndices;

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
            float particleRadius = PhysicsUtil.sqrt3 * 0.5f * SurfaceVoxelizer.m_voxelSize;

            m_positions = new Vector3[particlePositions.Count];
            //orientations = new Quaternion[particlePositions.Count];
            //restPositions = new Vector4[particlePositions.Count];
            //restOrientations = new Quaternion[particlePositions.Count];
            //velocities = new Vector3[particlePositions.Count];
            //angularVelocities = new Vector3[particlePositions.Count];
            m_invMasses = new float[particlePositions.Count];
            //invRotationalMasses = new float[particlePositions.Count];
            //principalRadii = new Vector3[particlePositions.Count];
            m_radius = new float[particlePositions.Count];
            //filters = new int[particlePositions.Count];
            m_colors = new Color[particlePositions.Count];

            //m_ActiveParticleCount = particlePositions.Count;

            for (int i = 0; i < particlePositions.Count; ++i)
            {
                // Perform ellipsoid fitting:
                List<Vector3> neighborhood = new List<Vector3>();

                Vector3 centroid = particlePositions[i];
                Quaternion orientation = Quaternion.identity;
                Vector3 principalValues = Vector3.one * particleRadius;

                // Calculate high-def voxel neighborhood extents:
                var anisotropyNeighborhood = Vector3.one * ShapeVoxelizer.m_voxelSize * PhysicsUtil.sqrt3 * 2;
                Vector3Int min = ShapeVoxelizer.GetPointVoxel(centroid - anisotropyNeighborhood) - ShapeVoxelizer.Origin;
                Vector3Int max = ShapeVoxelizer.GetPointVoxel(centroid + anisotropyNeighborhood) - ShapeVoxelizer.Origin;

                for (int nx = min.x; nx <= max.x; ++nx)
                    for (int ny = min.y; ny <= max.y; ++ny)
                        for (int nz = min.z; nz <= max.z; ++nz)
                        {
                            if (ShapeVoxelizer.VoxelExists(nx, ny, nz) &&
                                ShapeVoxelizer[nx, ny, nz] != MeshVoxelizer.Voxel.Outside)
                            {
                                Vector3 voxelCenter = ShapeVoxelizer.GetVoxelCenter(new Vector3Int(nx, ny, nz));
                                neighborhood.Add(voxelCenter);
                            }
                        }

                // distance field normal:
                Vector3 dfnormal = m_distanceField.SampleFiltered(centroid.x, centroid.y, centroid.z);

                // if the distance field normal isn't robust enough, use average vertex normal:
                if (Vector3.Dot(dfnormal, particleNormals[i]) < 0.75f)
                    dfnormal = particleNormals[i];

                var maxAnisotropy = 0;
                // if the particle has a non-empty neighborhood, perform ellipsoidal fitting:
                if (neighborhood.Count > 0)
                    PhysicsUtil.GetPointCloudAnisotropy(neighborhood, maxAnisotropy, particleRadius, dfnormal, ref centroid, ref orientation, ref principalValues);

                //invRotationalMasses[i] = invMasses[i] = 1.0f;
                m_positions[i] = Vector3.Lerp(particlePositions[i], centroid, m_smoothing);
                //restPositions[i] = positions[i];
                //restPositions[i][3] = 1; // activate rest position.
                //orientations[i] = orientation;
                //restOrientations[i] = orientation;
                //principalRadii[i] = principalValues;
                m_radius[i] = particleRadius;
                //filters[i] = ObiUtils.MakeFilter(ObiUtils.CollideWithEverything, 1);
                m_colors[i] = Color.white;

                if (i % 100 == 0)
                    yield return new CoroutineJob.ProgressInfo("ParticleTopology: generating particles...", i / (float)particlePositions.Count);
            }
        }

        /// <summary>
        /// 在ShapeVoxelizer的m_pathFinder中计算邻居体素中粒子到当前粒子的距离，
        /// 对于小于阈值的建立连接
        /// </summary>
        /// <param name="voxelizer"></param>
        /// <param name="particle"></param>
        /// <param name="particles"></param>
        /// <param name="allowed"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="neighborhood"></param>
        /// <param name="clusterSize"></param>
        /// <param name="cluster"></param>
        protected void ConnectToNeighborParticles(MeshVoxelizer voxelizer, int particle, List<Vector3> particles, List<ParticleType> allowed, int x, int y, int z, Vector3Int[] neighborhood, float clusterSize, List<int> cluster)
        {
            Vector3Int startVoxel = ShapeVoxelizer.GetPointVoxel(particles[particle]) - ShapeVoxelizer.Origin;
            startVoxel = m_pathFinder.FindClosestNonEmptyVoxel(startVoxel).m_coordinates;

            for (int j = 0; j < neighborhood.Length; ++j)
            {
                var voxel = neighborhood[j];
                int index = voxelizer.GetVoxelIndex(x + voxel.x, y + voxel.y, z + voxel.z);
                //m_voxelToParticles是voxelizer产生的，不是ShapeVoxelizer
                //可能是更加粗糙的
                foreach (var neighbor in m_voxelToParticles[index])
                {
                    if (!allowed.Contains(m_particleTypes[particle] | m_particleTypes[neighbor]))
                        continue;

                    Vector3Int endVoxel = ShapeVoxelizer.GetPointVoxel(particles[neighbor]) - ShapeVoxelizer.Origin;
                    endVoxel = m_pathFinder.FindClosestNonEmptyVoxel(endVoxel).m_coordinates;
                    //ShapeVoxelizer应该是比其他两个解析度要高，返回的距离更准确
                    var path = m_pathFinder.FindPath(startVoxel, endVoxel);
                    float geodesicDistance = path.m_distance;

                    if (geodesicDistance <= clusterSize)
                        cluster.Add(neighbor);
                }
            }
        }

        /// <summary>
        /// 根据体素距离在粒子间建立连接关系
        /// </summary>
        /// <param name="voxelizer"></param>
        /// <param name="particles"></param>
        /// <param name="connectivity"></param>
        /// <param name="allowed"></param>
        /// <returns></returns>
        protected IEnumerator CreateClustersFromVoxels(MeshVoxelizer voxelizer, List<Vector3> particles, VoxelConnectivity connectivity, List<ParticleType> allowed)
        {
            float clusterSize = PhysicsUtil.sqrt3 * voxelizer.m_voxelSize * 1.5f;

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

                m_colorizer.AddConstraint(cluster.ToArray());

                if (i % 100 == 0)
                    yield return new CoroutineJob.ProgressInfo("ParticleTopology: generating shape matching clusters...", i / (float)voxelizer.voxelCount);
            }
        }

        /// <summary>
        /// 建立体素和它所包含粒子的映射
        /// </summary>
        /// <param name="voxelizer"></param>
        /// <param name="particles"></param>
        /// <returns></returns>
        private IEnumerator InsertParticlesIntoVoxels(MeshVoxelizer voxelizer, List<Vector3> particles)
        {
            m_voxelToParticles = new List<int>[voxelizer.voxelCount];
            for (int i = 0; i < m_voxelToParticles.Length; ++i)
                m_voxelToParticles[i] = new List<int>(4);

            for (int i = 0; i < particles.Count; ++i)
            {
                Vector3Int voxel = voxelizer.GetPointVoxel(particles[i]) - voxelizer.Origin;
                int index = voxelizer.GetVoxelIndex(voxel.x, voxel.y, voxel.z);

                m_voxelToParticles[index].Add(i);

                if (i % 100 == 0)
                    yield return new CoroutineJob.ProgressInfo("ParticleTopology: inserting particles into voxels...", i / (float)particles.Count);
            }
        }

        /// <summary>
        /// 在体素位置上产生粒子
        /// </summary>
        /// <param name="voxelizer"></param>
        /// <param name="particles"></param>
        /// <param name="voxelType">关注体素类型</param>
        /// <param name="pType"></param>
        /// <returns></returns>
        private IEnumerator VoxelSampling(MeshVoxelizer voxelizer, List<Vector3> particles, MeshVoxelizer.Voxel voxelType, ParticleType pType)
        {
            int i = 0;

            for (int x = 0; x < voxelizer.m_resolution.x; ++x)
                for (int y = 0; y < voxelizer.m_resolution.y; ++y)
                    for (int z = 0; z < voxelizer.m_resolution.z; ++z)
                    {
                        if ((voxelizer[x, y, z] & voxelType) != 0)
                        {
                            var voxel = new Vector3Int(x, y, z);
                            Vector3 voxelCenter = voxelizer.GetVoxelCenter(voxel);

                            particles.Add(voxelCenter);
                            m_particleTypes.Add(pType);
                        }
                        if (++i % 1000 == 0)
                            yield return new CoroutineJob.ProgressInfo("ParticleTopology: sampling voxels...", i / (float)voxelizer.voxelCount);
                    }
        }

        private IEnumerator VoxelizeForVolumeSampling(Vector3 boundsSize)
        {
            float longestSide = Mathf.Max(Mathf.Max(boundsSize.x, boundsSize.y), boundsSize.z);
            float size = longestSide / m_volumeResolution;

            VolumeVoxelizer = new MeshVoxelizer(m_inputMesh, size);
            var voxelizeCoroutine = VolumeVoxelizer.Voxelize(m_localTransform, true);
            while (voxelizeCoroutine.MoveNext())
                yield return voxelizeCoroutine.Current;

            VolumeVoxelizer.BoundaryThinning();
        }

        private IEnumerator VoxelizeForShapeAnalysis(Vector3 boundsSize)
        {
            // Calculate voxel size: 
            float longestSide = Mathf.Max(Mathf.Max(boundsSize.x, boundsSize.y), boundsSize.z);
            float size = longestSide / m_shapeResolution;

            // Voxelize mesh and calculate discrete distance field:
            ShapeVoxelizer = new MeshVoxelizer(m_inputMesh, size);
            var voxelizeCoroutine = ShapeVoxelizer.Voxelize(m_localTransform);
            while (voxelizeCoroutine.MoveNext())
                yield return voxelizeCoroutine.Current;

            ShapeVoxelizer.BoundaryThinning();

            // Generate distance field:
            m_distanceField = new VoxelDistanceField(ShapeVoxelizer);
            var dfCoroutine = m_distanceField.JumpFloodUpdateDF();
            while (dfCoroutine.MoveNext())
                yield return dfCoroutine.Current;

            // Create path finder:
            m_pathFinder = new VoxelPathFinder(ShapeVoxelizer);
        }

        private IEnumerator VoxelizeForSurfaceSampling(Vector3 boundsSize)
        {
            float longestSide = Mathf.Max(Mathf.Max(boundsSize.x, boundsSize.y), boundsSize.z);
            float size = longestSide / m_surfaceResolution;

            SurfaceVoxelizer = new MeshVoxelizer(m_inputMesh, size);
            var voxelizeCoroutine = SurfaceVoxelizer.Voxelize(m_localTransform, true);
            while (voxelizeCoroutine.MoveNext())
                yield return voxelizeCoroutine.Current;

            SurfaceVoxelizer.BoundaryThinning();
        }

        /// <summary>
        /// 顶点采样产生粒子
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="particlePositions"></param>
        /// <returns></returns>
        private IEnumerator VertexSampling(Vector3[] vertices, List<Vector3> particlePositions)
        {
            float particleRadius = PhysicsUtil.sqrt3 * 0.5f * SurfaceVoxelizer.m_voxelSize;

            for (int i = 0; i < vertices.Length; ++i)
            {
                bool valid = true;
                //避免过密
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
                    m_particleTypes.Add(ParticleType.Surface);
                }

                if (i % 100 == 0)
                    yield return new CoroutineJob.ProgressInfo("ParticleTopology: sampling surface...", i / (float)vertices.Length);
            }
        }

        private IEnumerator MapVerticesToParticles(Vector3[] vertices, Vector3[] normals, List<Vector3> particlePositions, List<Vector3> particleNormals)
        {
            m_vertexToParticle = new int[vertices.Length];

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
                        m_vertexToParticle[i] = j;
                    }
                }
                //平均法线
                particleNormals[m_vertexToParticle[i]] += normals[i];

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
            //根据原始网格，产生粒子间连接关系
            for (int i = 0; i < meshTriangles.Length; i += 3)
            {
                int p1 = m_vertexToParticle[meshTriangles[i]];
                int p2 = m_vertexToParticle[meshTriangles[i + 1]];
                int p3 = m_vertexToParticle[meshTriangles[i + 2]];

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

                //输入是与i粒子相连接的粒子索引数组
                m_colorizer.AddConstraint(cluster.ToArray());
            }
        }
    }
#endif
}
