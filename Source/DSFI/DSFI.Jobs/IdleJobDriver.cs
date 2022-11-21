using Verse.AI;

namespace DSFI.Jobs;

public abstract class IdleJobDriver : JobDriver
{
    protected ReportStringState reportState;

    public override string GetReport()
    {
        if (job.def is not IdleJobDef idleJobDef)
        {
            return base.GetReport();
        }

        switch (reportState)
        {
            case ReportStringState.A:
                return ReportStringProcessed(idleJobDef.reportStringA);
            case ReportStringState.B:
                return ReportStringProcessed(idleJobDef.reportStringB);
            case ReportStringState.C:
                return ReportStringProcessed(idleJobDef.reportStringC);
            case ReportStringState.D:
                return ReportStringProcessed(idleJobDef.reportStringD);
            case ReportStringState.E:
                return ReportStringProcessed(idleJobDef.reportStringE);
            default:
                return base.GetReport();
        }
    }
}
