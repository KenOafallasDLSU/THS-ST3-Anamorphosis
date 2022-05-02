using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Data : MonoBehaviour
{
    public int nLevels; // to change
    public int[] scores; // time in milliseconds

    public Data(int nLevels)
    {
        this.nLevels = nLevels;
        this.scores = new int[nLevels];
        for (int i = 0; i < nLevels; i++)
        {
            scores[i] = 0;
        }
    }

    public void updateScore(int level, int score)
    {
        if (this.scores[level-1] > score || this.scores[level-1] == 0)
        {
            this.scores[level - 1] = score;
        }
    }
}
