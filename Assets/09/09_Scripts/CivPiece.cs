using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;


/// <summary>
/// ��������� CivPiece - ��� ���� "�������" ����������� � ���������� �������. 
/// ���� ����������� ���� � ��� ��������, ��� ������, ��� ��� ������� �� ��� �������� CivPiece
/// </summary>
public class CivPiece
{
    public int regionId;          //wmsk_id ���� �������, ��� ���� ���� ������
    public Population population;
    public Civilization belonging;

    public void Init(int wmsk_id, int startPop, Civilization from)         //
    {
        regionId = wmsk_id;
        population = new Population();
        population.Value = startPop;
        belonging = from;
    }
    public void addPieceToRegion(TM_Region region, CivPiece piece)
    {
        region.civPieces.Add(piece);
        
    }
}
