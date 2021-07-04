using System;
using System.Collections.Generic;

namespace ContentAwareResize
{
    // *****************************************
    // DON'T CHANGE CLASS OR FUNCTION NAME
    // YOU CAN ADD FUNCTIONS IF YOU NEED TO
    // *****************************************
    public class ContentAwareResize
    {
        public struct coord
        {
            public int row;
            public int column;
        }
        //========================================================================================================
        //Your Code is Here:
        //===================
        /// <summary>
        /// Develop an efficient algorithm to get the minimum vertical seam to be removed
        /// </summary>
        /// <param name="energyMatrix">2D matrix filled with the calculated energy for each pixel in the image</param>
        /// <param name="Width">Image's width</param>
        /// <param name="Height">Image's height</param>
        /// <returns>BY REFERENCE: The min total value (energy) of the selected seam in "minSeamValue" & List of points of the selected min vertical seam in seamPathCoord</returns>
        public void CalculateSeamsCost(int[,] energyMatrix, int Width, int Height, ref int minSeamValue, ref List<coord> seamPathCoord)
        {
                coord c=new coord();
               seamPathCoord= new List<coord>();
            List<coord> l = new List<coord>();
            int[,] costarray = new int[Height, Width];
           
            for (int i = 0; i < Width; i++)
            {
                costarray[0, i] = energyMatrix[0, i];

            }

            for (int row = 1; row < Height; row++)
            {
                for (int col = 0; col < Width; col++)
                {
                    if (col == 0)
                    {
                        costarray[row, col] = energyMatrix[row, col]+(Math.Min(costarray[(row - 1), (col)], costarray[(row - 1), (col + 1)]));

                    }
                  else if (col ==( Width - 1))
                    {
                        costarray[row, col] = energyMatrix[row, col]+(Math.Min(costarray[(row - 1), (col)], costarray[(row - 1), (col - 1)]));
                    }

                   else
                    {
               costarray[row, col] = energyMatrix[row, col] + (Math.Min(costarray[(row - 1), (col + 1)],(Math.Min(costarray[(row - 1), (col)], costarray[(row - 1), (col - 1)]) )));
                    }

                }
            }
      

            int msvcol = Width;
            int minnum = int.MaxValue;
            for (int i = 0; i < Width; i++)
            {
                if (costarray[Height - 1, i] < minnum)
                {
                    minnum = costarray[Height - 1, i];
                    minSeamValue = minnum;
                    msvcol = i;
                }
            }

            c.row = Height - 1; 
            c.column = msvcol;
            l.Add(c);
                
                for (int row = Height-2; row >= 0; row--)
                {
                int minm = 1000000000;
                int minmcol = 0;
               
                if (msvcol != 0)
                {
                    if(costarray[row,msvcol -1 ] < minm)
                    {
                        minmcol = msvcol - 1;
                        minm = costarray[row, msvcol - 1];
                    }
                }
                if (msvcol != (Width - 1))
                {
                    if (costarray[row, msvcol + 1] < minm)
                    {
                        minmcol = msvcol + 1;
                        minm = costarray[row, msvcol + 1];
                    }

                }
                if (costarray[row, msvcol] < minm)
                {
                    minmcol = msvcol ;
                    minm = costarray[row, msvcol];
                }
             
                c.row = row;
                c.column = minmcol;
                l.Add(c);
                msvcol = minmcol;
            }
             for(int i = l.Count - 1; i >= 0; i--)
            {
                seamPathCoord.Add(l[i]);
            }   

        }
      
        // *****************************************
        // DON'T CHANGE CLASS OR FUNCTION NAME
        // YOU CAN ADD FUNCTIONS IF YOU NEED TO 
        // *****************************************
        #region DON'TCHANGETHISCODE
        public MyColor[,] _imageMatrix;
        public int[,] _energyMatrix;
        public int[,] _verIndexMap;
        public ContentAwareResize(string ImagePath)
        {
            _imageMatrix = ImageOperations.OpenImage(ImagePath);
            _energyMatrix = ImageOperations.CalculateEnergy(_imageMatrix);
            int _height = _energyMatrix.GetLength(0);
            int _width = _energyMatrix.GetLength(1);
        }
        public void CalculateVerIndexMap(int NumberOfSeams, ref int minSeamValueFinal, ref List<coord> seamPathCoord)
        {
            int Width = _imageMatrix.GetLength(1);
            int Height = _imageMatrix.GetLength(0);

            int minSeamValue = -1;
            _verIndexMap = new int[Height, Width];
            for (int i = 0; i < Height; i++)
                for (int j = 0; j < Width; j++)
                    _verIndexMap[i, j] = int.MaxValue;

            bool[] RemovedSeams = new bool[Width];
            for (int j = 0; j < Width; j++)
                RemovedSeams[j] = false;

            for (int s = 1; s <= NumberOfSeams; s++)
            {
                CalculateSeamsCost(_energyMatrix, Width, Height, ref minSeamValue, ref seamPathCoord);
                minSeamValueFinal = minSeamValue;

                //Search for Min Seam # s
                int Min = minSeamValue;

                //Mark all pixels of the current min Seam in the VerIndexMap
                if (seamPathCoord.Count != Height)
                 //   throw new Exception("You selected WRONG SEAM");
                for (int i = Height - 1; i >= 0; i--)
                {
                    if (_verIndexMap[seamPathCoord[i].row, seamPathCoord[i].column] != int.MaxValue)
                    {
                        string msg = "overalpped seams between seam # " + s + " and seam # " + _verIndexMap[seamPathCoord[i].row, seamPathCoord[i].column];
                       // throw new Exception(msg);
                    }
                    _verIndexMap[seamPathCoord[i].row, seamPathCoord[i].column] = s;
                    //remove this seam from energy matrix by setting it to max value
                    _energyMatrix[seamPathCoord[i].row, seamPathCoord[i].column] = 100000;
                }

                //re-calculate Seams Cost in the next iteration again
            }
        }
        public void RemoveColumns(int NumberOfCols)
        {
            int Width = _imageMatrix.GetLength(1);
            int Height = _imageMatrix.GetLength(0);
            _energyMatrix = ImageOperations.CalculateEnergy(_imageMatrix);

            int minSeamValue = 0;
            List<coord> seamPathCoord = null;
            //CalculateSeamsCost(_energyMatrix,Width,Height,ref minSeamValue, ref seamPathCoord);
            CalculateVerIndexMap(NumberOfCols, ref minSeamValue, ref seamPathCoord);

            MyColor[,] OldImage = _imageMatrix;
            _imageMatrix = new MyColor[Height, Width - NumberOfCols];
            for (int i = 0; i < Height; i++)
            {
                int cnt = 0;
                for (int j = 0; j < Width; j++)
                {
                    if (_verIndexMap[i, j] == int.MaxValue)
                        _imageMatrix[i, cnt++] = OldImage[i, j];
                }
            }

        }
        #endregion
        }
    }


