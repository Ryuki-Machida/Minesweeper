using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Field : MonoBehaviour
{
    [SerializeField] Cell m_cellPrefab;
    [SerializeField] GridLayoutGroup m_container = null;
    [SerializeField] int m_row = 3;
    [SerializeField] int m_col = 3;
    [SerializeField] int m_bomb = 5;
    int m_flag = 0; //���̗����Ă��鐔
    int m_bombFlag = 0; //���e�Z���Ɋ��������Ă��鐔

    private Cell[,] m_cells;
    [SerializeField] Text m_text = null;
    [SerializeField] GameObject m_panel = null;

    private bool firstPush = false;

    void Start()
    {
        if (m_col < m_row)
        {
            m_container.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            m_container.constraintCount = m_row;
        }
        else
        {
            m_container.constraint = GridLayoutGroup.Constraint.FixedRowCount;
            m_container.constraintCount = m_col;
        }
        m_cells = new Cell[m_row, m_col];

        //�Z���̍쐬
        for (int col = 0; col < m_cells.GetLength(1); col++)
        {
            for (int row = 0; row < m_cells.GetLength(0); row++)
            {
                var cell = Instantiate(m_cellPrefab);
                var parent = m_container.transform;
                cell.transform.SetParent(parent);
                m_cells[row, col] = cell;
                cell.GetCoordinate(row, col);
            }
        }

        //���e
        for (var i = 0; i < m_bomb; i++)
        {
            if (m_row * m_col == 1) // ���e���}�X���ɒB�������߂�
            {
                break;
            }
            var r = Random.Range(0, m_row); //���̒��I
            var c = Random.Range(0, m_col); //�c�̒��I
            var cells = m_cells[r, c];
            if (cells.CellState != CellState.Mine) //�I�΂ꂽ�Z�������e�łȂ�������
            {
                cells.CellState = CellState.Mine; // ���e��n��
            }
            else
            {
                i--; // �����łȂ������璊�I����蒼��
            }
        }

        for (int col = 0; col < m_cells.GetLength(1); col++)
        {
            for (int row = 0; row < m_cells.GetLength(0); row++)
            {
                if (m_cells[row, col].CellState != CellState.Mine)
                {
                    continue;
                }
                else
                {
                    for (int c = -1; c < 2; c++)
                    {
                        for (int r = -1; r < 2; r++)
                        {
                            if (row + r == -1 || col + c == -1 || row + r == m_cells.GetLength(0) || col + c == m_cells.GetLength(1))
                            {
                                continue;
                            }
                            else if (m_cells[row + r, col + c].CellState != CellState.Mine)
                            {
                                m_cells[row + r, col + c].CellState += 1;
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// �J�����Z���̎���ɋ󔒂����邩���ׂ�
    /// </summary>
    public void CellCoordinate(int ro, int co)
    {
        for (int c = -1; c < 2; c++)
        {
            for (int r = -1; r < 2; r++)
            {
                if (ro + r == -1 || co + c == -1 || ro + r == m_cells.GetLength(0) || co + c == m_cells.GetLength(1))
                {
                    continue;
                }
                else if (r == 0 && c == 0)
                {
                    continue;
                }
                else if (m_cells[ro + r, co + c].Status != Status.Open)
                {
                    if (m_cells[ro + r, co + c].CellState == CellState.None)
                    {
                        if (m_cells[ro + r, co + c].Status == Status.Flag) // ����������W�J���Ȃ�
                        {
                            continue;
                        }
                        m_cells[ro + r, co + c].Status = (Status)3;
                    }
                }
            }
        }
    }

    /// <summary>
    /// �󔒂����łȂ��Ȃ�W�J����
    /// </summary>
    public void CellOpen(int ro, int co)
    {
        for (int c = -1; c < 2; c++)
        {
            for (int r = -1; r < 2; r++)
            {
                if (ro + r == -1 || co + c == -1 || ro + r == m_cells.GetLength(0) || co + c == m_cells.GetLength(1))
                {
                    continue;
                }
                else if (r == 0 && c == 0)
                {
                    continue;
                }
                else if (m_cells[ro + r, co + c].Status != Status.Open)
                {
                    if (m_cells[ro + r, co + c].Status == Status.Flag)
                    {
                        continue;
                    }
                    m_cells[ro + r, co + c].Status = (Status)3;
                }
            }
        }
    }

    /// <summary>
    /// �������Ă�ꂽ�����󂯎��
    /// </summary>
    public void Flag(int f)
    {
        m_flag += f;
    }

    /// <summary>
    /// ���e�Z���Ɋ����u���ꂽ���𒲂ׂ�
    /// </summary>
    public void BombFlag(int f)
    {
        m_bombFlag += f;
        if (m_bombFlag == m_bomb)
        {
            GameClear();
        }
    }

    public void BombOtherThan()
    {
        int bombOtherThan = 0;
        foreach (var i in m_cells)
        {
            if (i.Status == Status.Open)
            {
                bombOtherThan++;
            }
        }
        if (m_row * m_col - bombOtherThan == m_bomb)
        {
            GameClear();
        }
    }

    /// <summary>
    /// �����̏���𐧌�
    /// </summary>
    public bool Fl()
    {
        if (m_flag < m_bomb)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void GameClear()
    {
        m_text.text = "�Q�[���N���A";
        foreach (var i in m_cells)
        {
            i.GetComponent<Cell>().GameEnd();
            m_panel.SetActive(true);
        }
    }

    public void GameOver()
    {
        m_text.text = "�Q�[���I�[�o�[";
        foreach (var i in m_cells)
        {
            i.GetComponent<Cell>().GameEnd();
            m_panel.SetActive(true);
        }
    }

    public void Reset()
    {
        if (!firstPush)
        {
            SceneManager.LoadScene("Minesweeper");
            firstPush = true;
        }
    }
}
