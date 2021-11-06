using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;

/// <summary>
/// 将csv解析为string grid的类
/// csv规范参考：
/// https://blog.csdn.net/icycode/article/details/80043956
/// https://tools.ietf.org/html/rfc4180
/// </summary>
public class CsvParser
{
    /// <summary>
    /// 域分界符
    /// </summary>
    private const char FIELD_DELIMITER = ',';   // A one-character string used to separate fields.
    /// <summary>
    /// 当csv某个字段用双引号括起来且字段中内容中还包含双引号时，是否在该引号前面再添加一个双引号进行转义
    /// </summary>
    private const bool DOUBLEQUOTE = true; // if true, quote in quote field need double
    /// <summary>
    /// 双引号
    /// </summary>
    private const char QUOTECHAR = '"';   // the character to quote
    /// <summary>
    /// 新行符号
    /// </summary>
    private const char NEWLINE = '\n';  // line feeder character
    /// <summary>
    /// 回车符号
    /// </summary>
    private const char CARRIAGERETURN = '\r';

    /// <summary>
    /// 列数
    /// </summary>
    private int Column;

    /// <summary>
    /// 行数
    /// </summary>
    private int Row;

    /// <summary>
    /// 原始数据
    /// </summary>
    private string RawContent;

    /// <summary>
    /// 当前位置
    /// </summary>
    private int m_pos;

    /// <summary>
    /// 解析后的的string网格
    /// </summary>
    private List<string[]> m_grid = new List<string[]>();

    private void Clear()
    {
        Column = -1;
        Row = -1;
        RawContent = "";
        m_pos = 0;
        m_grid.Clear();
    }

    /// <summary>
    /// 预览下个位置的字符
    /// </summary>
    /// <returns></returns>
    private char Peek()
    {
        if (m_pos < RawContent.Length - 1) return RawContent[m_pos + 1];
        else return char.MinValue;
    }

    /// <summary>
    /// 将行数据加入网格数据中
    /// </summary>
    /// <param name="rows"></param>
    /// <param name="isFirstRow"></param>
    private void AddRow(List<string> rows, bool isFirstRow = false)
    {
        m_grid.Add(rows.ToArray());
        if (isFirstRow)
        {
            Column = rows.Count;
            Row = 1;
        }
        else
        {
            Row++;
        }
    }

    /// <summary>
    /// 解析方法
    /// </summary>
    /// <param name="content"></param>
    private void _Parse(string content)
    {
        Clear();
        RawContent = content;
        List<string> line = new List<string>();
        StringBuilder buffer = new StringBuilder(1024);

        bool isInQuote = false;

        //将当前位置的字符加入到field缓冲中
        Action AddCurrent = () =>
        {
            buffer.Append(RawContent[m_pos]);
        };

        //将field从缓冲中提取，加入到行数据中
        Action AddField = () =>
        {
            line.Add(buffer.ToString());
            buffer.Length = 0;
        };
        //将行数据加入到网格数据中
        Action AddToGrid = () =>
        {
            AddRow(line, Row == -1);
            line.Clear();
        };

        while (m_pos < RawContent.Length)
        {
            if (RawContent[m_pos] == QUOTECHAR)
            {
                if (!isInQuote)
                {
                    isInQuote = true;
                }
                else
                {
                    if (Peek() == FIELD_DELIMITER)//引号后接一个逗号作为该field的结束标志
                    {
                        AddField();
                        m_pos++;
                        isInQuote = false;
                    }
                    else if (Peek() == NEWLINE || Peek() == CARRIAGERETURN)//如果引号括起来的这个filed是该行的最后一个field
                    {
                        isInQuote = false; //转入接下来的meet line terminator的流程
                    }
                    else
                    {
                        //引号括起来的这个filed中出现的引号是否需要转义
                        if (DOUBLEQUOTE)
                        {
                            if (Peek() != QUOTECHAR)
                            {
                                throw new Exception("quote character in not double in quote field");
                            }
                            m_pos++;
                        }
                        AddCurrent();
                    }
                }
            }
            else if (RawContent[m_pos] == FIELD_DELIMITER)
            {
                if (!isInQuote)
                {
                    AddField();
                }
                else
                {
                    //如果逗号在分号中出现不会被当作field分隔符
                    AddCurrent();
                }
            }
            else if (RawContent[m_pos] == NEWLINE || RawContent[m_pos] == CARRIAGERETURN) // meet line terminator
            {
                if (!isInQuote)
                {
                    // skip /r/n
                    if (RawContent[m_pos] == CARRIAGERETURN && Peek() == NEWLINE)
                    {
                        m_pos++;
                    }
                    AddField();
                    AddToGrid();
                }
                else
                {
                    AddCurrent();
                }
            }
            else
            {
                AddCurrent();
            }
            m_pos++;
        }
        if (line.Count > 0)
        {
            // when last character is not line terminator
            AddField();
            AddToGrid();
        }
    }

    public static List<string[]> Parse(string content)
    {
        CsvParser parse = new CsvParser();
        parse._Parse(content);
        //parse.DebugOutputmGrid();
        return parse.m_grid;
    }

    private void DebugOutputmGrid()
    {
        StringBuilder textOutput = new StringBuilder("****Dump CSV Table****\n", 1024);
        for (int x = 0; x < Row; x++)
        {
            for (int y = 0; y < Column; y++)
            {
                textOutput.Append(m_grid[x][y]);
                textOutput.Append("|");
            }
            textOutput.Append("\n");
        }
        Debug.Log(textOutput.ToString());
    }

}
