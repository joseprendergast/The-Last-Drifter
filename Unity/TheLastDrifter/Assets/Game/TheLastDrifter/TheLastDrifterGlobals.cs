public partial class GlobalScript
{
    public enum eDrifterCaseProgress
    {
        NewGame,
        EnteredAlley,
        SawSeveredHand,
        FoundBloodDrain,
        UnlockedLavatory,
        EnteredBar,
        ReachedMorningZoo
    }

    public eDrifterCaseProgress m_drifterProgress = eDrifterCaseProgress.NewGame;
    public bool m_sawSeveredHand = false;
    public bool m_checkedRain = false;
    public bool m_checkedDrain = false;
    public bool m_foundBloodLavatory = false;
    [System.NonSerialized] public bool m_enteredBar = false;
    [System.NonSerialized] public bool m_talkedToBartender = false;
    [System.NonSerialized] public bool m_checkedDarts = false;
    [System.NonSerialized] public bool m_checkedPoolTable = false;

    public void ResetDrifterCase()
    {
        m_drifterProgress = eDrifterCaseProgress.NewGame;
        m_sawSeveredHand = false;
        m_checkedRain = false;
        m_checkedDrain = false;
        m_foundBloodLavatory = false;
        m_enteredBar = false;
        m_talkedToBartender = false;
        m_checkedDarts = false;
        m_checkedPoolTable = false;
    }
}
