using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ConstrainBase
{
    protected ISolverEnv m_solveEnv;
    public ConstrainType m_type;

    public void SetSolveEnv(ISolverEnv solveEnv)
    {
        m_solveEnv = solveEnv;
    }

    public ConstrainBase(ConstrainType type)
    {
        m_type = type;
    }

    public virtual void Solve(float dt) { }
}
