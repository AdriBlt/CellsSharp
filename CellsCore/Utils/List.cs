using System;

namespace CellsCore.Utils
{
    static class List
    {
        public static T[][] CreateDefaultMatrix<T> (
            int width,
            int height,
            Func<int, int, T> getDefaultValue)
        {
            var matrix = new T[height][];
            for (int i = 0; i < height; i++)
            {
                matrix[i] = new T[width];
                for (int j = 0; j < width; j++)
                {
                    matrix[i][j] = getDefaultValue(i, j);
                }
            }

            return matrix;
        }
    }
}
