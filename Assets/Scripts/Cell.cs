using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CellState
{
    None = 0,
    One = 1,
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5,
    Six = 6,
    Seven = 7,
    Eight = 8,

    Mine = -1,
}

public enum Status
{
    Close = 0,
    Open = 1,
    Flag = 2,
    NoneOpen = 3,
}

public class Cell : MonoBehaviour
{
    [SerializeField] private Text m_view = null;
    [SerializeField] private CellState m_cellState = CellState.None;
    [SerializeField] Button m_button = null;
    public int row;
    public int col;
    [SerializeField] private Text m_hata = null;
    [SerializeField] private Status m_status = Status.Close;
    [SerializeField] Image m_image = null;

    private GameObject mine;

    public CellState CellState
    {
        get => m_cellState;
        set
        {
            m_cellState = value;
            OnCellStateChanged();
        }
    }
    public Status Status
    {
        get => m_status;
        set
        {
            m_status = value;
            OnCellflagChanged();
        }
    }

    void Start()
    {
        mine = GameObject.Find("Minesweeper");
    }

    private void OnValidate()
    {
        OnCellStateChanged();
        OnCellflagChanged();
    }

    private void OnCellStateChanged()
    {
        if (m_view == null)
        {
            return;
        }
        if (m_cellState == CellState.None)
        {
            m_view.text = "";
        }
        else if (m_cellState == CellState.Mine)
        {
            m_view.text = "X";
            m_view.color = Color.red;
        }
        else
        {
            m_view.text = ((int)m_cellState).ToString();
            m_view.color = Color.blue;
        }
    }

    private void OnCellflagChanged()
    {
        if (m_status == Status.Close)
        {
            m_hata.text = "";
        }
        else if (m_status == Status.Flag)
        {
            m_hata.text = "●";
        }
        else if (m_status == Status.Open)
        {
            Open();
        }
        else if (m_status == Status.NoneOpen)
        {
            NoneOpen();
        }
    }

    public void GetCoordinate(int r, int c)
    {
        row = r;
        col = c;
    }

    /// <summary>
    /// セルの展開
    /// </summary>
    public void Open()
    {
        if (this.m_status != Status.Flag)
        {
            m_button.gameObject.SetActive(false);
            this.m_status = Status.Open;
            Debug.Log(row + " " + col + "が開いた");
            if (this.m_cellState == CellState.Mine)
            {
                m_image.color = Color.red;
                mine.GetComponent<Field>().GameOver();
            }
            else
            {
                Field field = FindObjectOfType<Field>();
                if (field)
                {
                    field.CellCoordinate(row, col);
                    field.BombOtherThan();
                }
            }
        }
    }

    public void NoneOpen()
    {
        m_button.gameObject.SetActive(false);
        this.m_status = Status.Open;
        Debug.Log(row + " " + col + "が開いた");
        Field field = FindObjectOfType<Field>();
        if (field)
        {
            if (this.m_cellState == CellState.None)
            {
                field.CellOpen(row, col);
            }
        }
    }

    /// <summary> 旗を立てる </summary>
    public void Flag()
    {
        if (Input.GetMouseButtonUp(1))
        {
            Debug.Log("hata");
            if (this.m_status == Status.Close)
            {
                bool f = mine.GetComponent<Field>().Fl();
                if (f == true)
                {
                    m_hata.text = "●";
                    this.m_status = Status.Flag;
                    Field field = FindObjectOfType<Field>();
                    if (field)
                    {
                        field.Flag(1);
                    }
                    if (this.m_cellState == CellState.Mine)
                    {
                        field.BombFlag(1);
                    }
                }
                else
                {
                    Debug.Log("旗なし");
                }
            }
            else
            {
                m_hata.text = "";
                this.m_status = Status.Close;
                Field field = FindObjectOfType<Field>();
                if (field)
                {
                    field.Flag(-1);
                }
                if (this.m_cellState == CellState.Mine)
                {
                    field.BombFlag(-1);
                }
            }
        }
    }

    public void GameEnd()
    {
        m_button.gameObject.SetActive(false);
    }
}
