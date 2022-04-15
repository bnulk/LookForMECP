using System;
using System.Collections.Generic;
using System.Text;
using ChemKun.LinearAlgebra;

namespace ChemKun.Estimate
{
    class EstimateHessian
    {
        int dim;                     //维数，
        double[,] lastQ;             //上一步的坐标， dim行1列
        double[,] lastGrad;          //上一步的梯度， dim行1列
        double[,] lastHessian;       //上一步的Hessian阵， dim行dim列
        double[,] q;                 //本步坐标， dim行1列
        double[,] grad;              //本步梯度， dim行1列

        #region 构造函数
        public EstimateHessian(int dim, double[] lastQ, double[] lastGrad, double[,] lastHessian, double[] q, double[] grad)
        {
            this.dim = dim;
            this.lastQ = new double[dim, 1];
            this.lastGrad = new double[dim, 1];
            this.q = new double[dim, 1];
            this.grad = new double[dim, 1];
            this.lastHessian = new double[dim, dim];

            for (int i = 0; i < dim; i++)
            {
                this.lastQ[i, 0] = lastQ[i];
                this.lastGrad[i, 0] = lastGrad[i];
                this.q[i, 0] = q[i];
                this.grad[i, 0] = grad[i];
                for (int j = 0; j < dim; j++)
                {
                    this.lastHessian[i, j] = lastHessian[i, j];
                }
            }
        }
        #endregion

        public double[,] BFGS()
        {
            double[,] Hessian;
            BnulkMatrix secondItem;
            BnulkMatrix thirdItem;
            BnulkMatrix Pk;
            BnulkMatrix Kk;
            double denominator;                    //分母，第二项和第三项分母都是数字
            BnulkMatrix lastQMatrix = new BnulkMatrix(lastQ);
            BnulkMatrix lastGradMatrix = new BnulkMatrix(lastGrad);
            BnulkMatrix lastHessianMatrix = new BnulkMatrix(lastHessian);
            BnulkMatrix qMatrix = new BnulkMatrix(q);
            BnulkMatrix gradMatrix = new BnulkMatrix(grad);

            Pk = lastGradMatrix - gradMatrix;                  //注意：本程序中所有梯度，都是-DE/DX
            Kk = qMatrix - lastQMatrix;
            //计算第二项
            secondItem = Pk * BnulkMatrix.Transpose(Pk);
            denominator = (BnulkMatrix.Transpose(Pk) * Kk)[0, 0];
            secondItem = secondItem * (1 / denominator);
            //计算第三项
            thirdItem = BnulkMatrix.Transpose(Kk) * lastHessianMatrix;
            thirdItem = Kk * thirdItem;
            thirdItem = lastHessianMatrix * thirdItem;
            denominator = (BnulkMatrix.Transpose(Kk) * lastHessianMatrix * Kk)[0, 0];
            thirdItem = thirdItem * (1 / denominator);

            //计算Hessian阵
            Hessian = (lastHessianMatrix + secondItem - thirdItem).dataTwoDimArray;
            return Hessian;
        }

        public double[,] Powell()
        {
            double[,] Hessian;
            BnulkMatrix parentheses;
            BnulkMatrix parentheses1;
            BnulkMatrix parentheses2;
            BnulkMatrix parentheses3;
            double parentheses3_middle;
            double KTK;
            BnulkMatrix Kk;
            BnulkMatrix Tk;
            double denominator;                               //分母，括号内第三项分母。
            BnulkMatrix lastQMatrix = new BnulkMatrix(lastQ);
            BnulkMatrix lastGradMatrix = new BnulkMatrix(lastGrad);
            BnulkMatrix lastHessianMatrix = new BnulkMatrix(lastHessian);
            BnulkMatrix qMatrix = new BnulkMatrix(q);
            BnulkMatrix gradMatrix = new BnulkMatrix(grad);

            Kk = qMatrix - lastQMatrix;
            Tk = lastGradMatrix - gradMatrix - lastHessianMatrix * Kk;                  //注意：本程序中所有梯度，都是-DE/DX
            parentheses1 = Tk * BnulkMatrix.Transpose(Kk);
            parentheses2 = Kk * BnulkMatrix.Transpose(Tk);
            denominator = (BnulkMatrix.Transpose(Kk) * Kk)[0, 0];
            parentheses3_middle = (BnulkMatrix.Transpose(Tk) * Kk)[0, 0] / denominator;
            parentheses3 = Kk * parentheses3_middle * BnulkMatrix.Transpose(Kk);
            KTK = (BnulkMatrix.Transpose(Kk) * Kk)[0, 0];
            parentheses = parentheses1 + parentheses2 - parentheses3;
            Hessian = (lastHessianMatrix + parentheses * (1 / KTK)).dataTwoDimArray;

            return Hessian;
        }
    }
}
