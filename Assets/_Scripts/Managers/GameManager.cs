using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private DefenderUIController _defenderUIcontroller;

    public GameObject turnTimerText;
    public GameObject currentTurnText;
    SimpleTimer _timer;

    private enum GameState { None, DefenderTurn, AttackerTurn, RoundState, End };

    private GameState _currentTurn = GameState.None;

    [SerializeField]
    public int attackerTurnLengthSeconds = 5;

    [SerializeField]
    public int defenderTurnLengthSeconds = 5;

    [SerializeField]
    public int roundLengthseconds = 5;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TryGetComponent(out _defenderUIcontroller);
        Time.timeScale = 1;
        _timer = GetComponent<SimpleTimer>();
        _timer.SetTimerMode(TimerMode.CountDown);
        AdvanceTurn();
    }

    // Update is called once per frame
    void Update()
    {
        turnTimerText.GetComponent<TMP_Text>().SetText(string.Format("{0:0.##}", _timer.TimeLeft()));
        if (_timer.TimeLeft() == 0)
        {
            AdvanceTurn();
        }
    }

    private void BeginDefenderTurn()
    {
        _defenderUIcontroller.EnableControls(true);
        _currentTurn = GameState.DefenderTurn;
        _timer.SetTimerLength(defenderTurnLengthSeconds);
        _timer.BeginTimer();
    }

    private void BeginAttackerTurn()
    {
        _defenderUIcontroller.EnableControls(false);
        _currentTurn = GameState.AttackerTurn;
        _timer.SetTimerLength(attackerTurnLengthSeconds);
        _timer.BeginTimer();
    }

    private void BeginRound()
    {
        _currentTurn = GameState.RoundState;
        _timer.SetTimerLength(roundLengthseconds);
        _timer.BeginTimer();
    }

    private void AdvanceTurn()
    {
        switch (_currentTurn)
        {
            case GameState.None:
                BeginDefenderTurn();
                break;
            case GameState.DefenderTurn:
                BeginAttackerTurn();
                break;
            case GameState.AttackerTurn:
                BeginRound();
                break;
            case GameState.RoundState:
                BeginDefenderTurn();
                break;
        }

        currentTurnText.GetComponent<TMP_Text>().SetText(_currentTurn.ToString());
    }


}
