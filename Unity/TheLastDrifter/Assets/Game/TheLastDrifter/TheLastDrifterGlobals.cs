public partial class GlobalScript
{
    public enum eDrifterCaseProgress
    {
        NewGame,
        EnteredAlley,
        SawSeveredHand,
        FoundBloodDrain,
        UnlockedLavatory,
        ReachedMorningZoo
    }

    public eDrifterCaseProgress m_drifterProgress = eDrifterCaseProgress.NewGame;
    public bool m_sawSeveredHand = false;
    public bool m_checkedRain = false;
    public bool m_checkedDrain = false;
    public bool m_foundBloodLavatory = false;

    public void ResetDrifterCase()
    {
        m_drifterProgress = eDrifterCaseProgress.NewGame;
        m_sawSeveredHand = false;
        m_checkedRain = false;
        m_checkedDrain = false;
        m_foundBloodLavatory = false;
    }
}
