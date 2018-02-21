using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sdproject
{
	internal class MathUtils
	{
		/// <summary>
		/// Максимальное количество миноров размера minorSize в квадратной матрице размера matrixSize.
		/// </summary>
		/// <param name="matrixSize"></param>
		/// <param name="minorSize"></param>
		/// <returns></returns>
		public static int MaxMinorAmount(int matrixSize, int minorSize)
		{
			Debug.Assert(matrixSize > 0);
			Debug.Assert(minorSize > 0);
			Debug.Assert(matrixSize >= minorSize);
			return simplexNumber(matrixSize - minorSize + 1, minorSize);
		}

		/// <summary>
		/// N-е симплексное число порядка r.
		/// </summary>
		/// <param name="n">Номер симплексного числа</param>
		/// <param name="r">Порядок симплексного числа</param>
		/// <returns></returns>
		private static int simplexNumber(int n, int r)
		{
			Debug.Assert(n >= 0);
			Debug.Assert(r > 0);
			int product = n;
			for (int i = 2; i <= r; i++)
			{
				product *= n + i - 1;
				product /= i;
			}

			return product;
		}
	}
}
