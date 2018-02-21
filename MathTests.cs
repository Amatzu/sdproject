using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace sdproject
{
	internal class MathTests
	{
		[Test, TestCase(5, 2, 10), TestCase(5, 3, 10)]
		public void MinorCountTest(int matrixSize, int minorSize, int expectedResult)
		{
			Assert.AreEqual(expectedResult, MathUtils.MaxMinorAmount(matrixSize, minorSize));
		}
	}
}
