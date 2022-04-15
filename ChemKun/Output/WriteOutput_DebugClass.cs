using System;
using System.Collections.Generic;
using System.Text;
using ChemKun.LinearAlgebra;

namespace ChemKun.Output
{
    static partial class WriteOutput
    {
        /// <summary>
        /// 检查矩阵是否为正交矩阵
        /// </summary>
        /// <param name="matrix1">待检查矩阵</param>
        /// <param name="matrix1">待检查矩阵的名字</param>
        public static void CheckOrthogonalMatrix(double[,] twoDimArray, string name)
        {
            m_Result.Clear();
            m_Result.Append("待检查矩阵" + name.ToString() + "\n");
            int dim = twoDimArray.GetLength(0);
            if(dim!= twoDimArray.GetLength(1))
            {
                m_Result.Append("不是方二维数组。");
                Write();
                return;
            }

            BnulkMatrix matrix1 = new BnulkMatrix(twoDimArray);
            BnulkMatrix matrix2 = BnulkMatrix.Transpose(matrix1);
            BnulkMatrix matrix3 = new BnulkMatrix(dim, dim);

            matrix3 = matrix2 * matrix1;

            for(int i=0;i<dim;i++)
            {
                for(int j=0;j<dim;j++)
                {
                    m_Result.Append(matrix3[i, j].ToString("0.00000").PadLeft(15));
                }
                m_Result.Append("\n");
            }
            Write();
        }

        /// <summary>
        /// 检查矩阵是否为正交矩阵
        /// </summary>
        /// <param name="matrix1">待检查矩阵</param>
        /// <param name="matrix1">待检查矩阵的名字</param>
        public static void CheckOrthogonalMatrix(BnulkMatrix matrix1, string name)
        {
            m_Result.Clear();
            m_Result.Append("**********     " + name.ToString() + "     **********" + "\n");
            int dim = matrix1.row;
            if (dim != matrix1.column)
            {
                m_Result.Append("不是方二维数组。");
                Write();
                return;
            }

            BnulkMatrix matrix2 = BnulkMatrix.Transpose(matrix1);
            BnulkMatrix matrix3 = new BnulkMatrix(dim, dim);

            matrix3 = matrix2 * matrix1;

            for (int i = 0; i < dim; i++)
            {
                for (int j = 0; j < dim; j++)
                {
                    m_Result.Append(matrix3[i, j].ToString("0.00000").PadLeft(15));
                }
                m_Result.Append("\n");
            }
            Write();
        }

        public static void ShowMatrix(BnulkMatrix twoDimArray, string name)
        {
            m_Result.Clear();
            m_Result.Append("**********     " + name.ToString() + "     **********" + "\n");

            int row = twoDimArray.row;
            int column = twoDimArray.column;

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    m_Result.Append(twoDimArray[i, j].ToString("0.00000").PadLeft(15));
                }
                m_Result.Append("\n");
            }
            Write();

            return;
        }

        public static void ShowMatrix(double[,] twoDimArray, string name)
        {
            m_Result.Clear();
            m_Result.Append("**********     " + name.ToString() + "     **********" + "\n");

            int row = twoDimArray.GetLength(0);
            int column = twoDimArray.GetLength(1);

            for(int i=0;i<row;i++)
            {
                for(int j=0;j<column;j++)
                {
                    m_Result.Append(twoDimArray[i, j].ToString("0.00000").PadLeft(15));
                }
                m_Result.Append("\n");
            }
            Write();

            return;
        }

        public static void ShowVector(BnulkVec vec, string name)
        {
            m_Result.Clear();
            m_Result.Append("**********     " + name.ToString() + "     **********" + "\n");

            for (int i = 0; i < vec.dim; i++)
            {
                m_Result.Append(vec[i].ToString("0.00000").PadLeft(15));
            }

            m_Result.Append("\n");
            Write();

            return;
        }
    }
}
