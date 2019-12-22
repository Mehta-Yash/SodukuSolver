using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        //array for storing output result 
        static int[,] output = new int[9, 9];

        //for initial input 
        List<int> inputList = new List<int>();

        //for final output
        List<int> outputList = new List<int>();

        /// <summary>
        /// For showing Home Page
        /// </summary>
        /// <returns>Home page</returns>
        public ActionResult Index()
        {

            ViewBag.Title = "Soduku Game";
            return View();
        }


        /// <summary>
        /// For filling board based on input string
        /// </summary>
        /// <returns>List of int of input string</returns>
        public JsonResult InitialBoard()
          {
            //Input string(change this to try different cases)
            string inputString = "4.....8.5.3..........7......2.....6.....8.4......1.......6.3.7.5..2.....1.4......";

            foreach (char ch in inputString)
            {
                int charToInt;
                //if "." then add 0 or add the number
                if (ch.ToString() == ".")
                {
                    inputList.Add(0);
                }
                else
                {
                    charToInt = (int)char.GetNumericValue(ch);
                    inputList.Add(charToInt);
                }
            }

            int[] inputListToArray = inputList.ToArray();

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    output[i, j] = inputListToArray[i * 9 + j];
                }
            }

            return Json(inputList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// For finding results based on input string
        /// </summary>
        /// <returns>List of int for result</returns>
        public JsonResult Result()
        {
            int N = output.GetLength(0);

            if (solveSudoku(output, N))
            {
                //Converting 2-d array to 1-d array
                int[] outputArr = new int[81];
                int i, j, k = 0;
                for (i = 0; i < 9; i++)
                {
                    for (j = 0; j < 9; j++)
                    {
                        outputArr[k++] = output[i, j];
                    }
                }

                //Convering array to list for output
                outputList = outputArr.ToList();
                return Json(outputList, JsonRequestBehavior.AllowGet);
                               
            }
            else
            {
                return Json("No solution! Something is wrong", JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// if the number we are placing is safe or not
        /// </summary>
        /// <param name="board">input recieved</param>
        /// <param name="row">row</param>
        /// <param name="col">column</param>
        /// <param name="num">current number</param>
        /// <returns>true/false based on checks</returns>
        public static bool isSafe(int[,] board,
                            int row, int col,
                            int num)
        {
            // row has the unique (row-clash) 
            for (int d = 0; d < board.GetLength(0); d++)
            {
                // if the number we are trying to  
                // place is already present in  
                // that row, return false; 
                if (board[row, d] == num)
                {
                    return false;
                }
            }

            // column has the unique numbers (column-clash) 
            for (int r = 0; r < board.GetLength(0); r++)
            {
                // if the number we are trying to 
                // place is already present in 
                // that column, return false; 
                if (board[r, col] == num)
                {
                    return false;
                }
            }

            // corresponding square has 
            // unique number (box-clash) 
            int sqrt = (int)Math.Sqrt(board.GetLength(0));
            int boxRowStart = row - row % sqrt;
            int boxColStart = col - col % sqrt;

            for (int r = boxRowStart;
                    r < boxRowStart + sqrt; r++)
            {
                for (int d = boxColStart;
                        d < boxColStart + sqrt; d++)
                {
                    if (board[r, d] == num)
                    {
                        return false;
                    }
                }
            }

            // if there is no clash, it's safe 
            return true;
        }


        /// <summary>
        /// Main method which solves given soduku
        /// </summary>
        /// <param name="board">input recieved</param>
        /// <param name="n">total rows</param>
        /// <returns>is soduku solved or not </returns>
        public static bool solveSudoku(int[,] board, int n)
        {
            int row = -1;
            int col = -1;
            bool isEmpty = true;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (board[i, j] == 0)
                    {
                        row = i;
                        col = j;

                        // we still have some remaining 
                        // missing values in Sudoku 
                        isEmpty = false;
                        break;
                    }
                }
                if (!isEmpty)
                {
                    break;
                }
            }

            // no empty space left 
            if (isEmpty)
            {
                return true;
            }

            // else for each-row backtrack 
            for (int num = 1; num <= n; num++)
            {
                if (isSafe(board, row, col, num))
                {
                    board[row, col] = num;
                    if (solveSudoku(board, n))
                    {                        
                        return true;
                    }
                    else
                    {
                        board[row, col] = 0; // replace it 
                    }
                }
            }
            return false;
        }
        
    }
}
