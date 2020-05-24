using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteToBoards
{
    public struct SpriteMap
    {
        public Color32[] pixels;

        public int rowNumber;
        public int coloumnNumber;

        public int totalCount;

        public List<SingleBoard> boards;
    }

    public struct SingleBoard
    {
        public Color32 boardColor;
        public Vector2 boardPosition;
    }

    public List<SpriteMap> spriteMaps;

    public SpriteToBoards(string spritesPath)
    {
        UnityEngine.Object[] sprites = Resources.LoadAll(spritesPath, typeof(Texture2D));
        // Texture2D[] sprites = Array.ConvertAll<object, Texture2D>(Resources.LoadAll(spritesPath), tex => (Texture2D)tex); 
        spriteMaps = new List<SpriteMap>();

        foreach(Texture2D tex in sprites)
        {
            //Set sprite map parameters;
            SpriteMap spriteMap = new SpriteMap();

            spriteMap.rowNumber = tex.height;
            spriteMap.coloumnNumber = tex.width;

            spriteMap.pixels = tex.GetPixels32();
            spriteMap.boards = new List<SingleBoard>(spriteMap.pixels.Length);

            int count = 0;
            for(int r = 0; r < spriteMap.rowNumber; r++)
            {
                for(int c = 0; c < spriteMap.coloumnNumber; c++)
                {
                    SingleBoard board = new SingleBoard();
                    board.boardColor = spriteMap.pixels[count];
                    board.boardPosition = new Vector2(r, c);
                    spriteMap.boards.Add(board);

                    count++;
                }
            }

            spriteMaps.Add(spriteMap);

        }        
    }

}
