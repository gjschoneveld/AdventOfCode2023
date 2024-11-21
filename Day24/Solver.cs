using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day24;

public class Solver
{
    public static Quotient[]? Solve(Quotient[,] coefficients, Quotient[] result)
    {
        // Combine coefficients and result in an augmented matrix
        Quotient[,] augmentedMatrix = new Quotient[coefficients.GetLength(0), coefficients.GetLength(1) + 1];

        for (int row = 0; row < coefficients.GetLength(0); row++)
        {
            for (int column = 0; column < coefficients.GetLength(1); column++)
            {
                augmentedMatrix[row, column] = coefficients[row, column];
            }

            augmentedMatrix[row, coefficients.GetLength(1)] = result[row];
        }

        // Create echelon
        List<(int row, int column)> pivots = new();

        int currentRow = 0;
        int currentColumn = 0;

        while (currentRow < augmentedMatrix.GetLength(0) && currentColumn < augmentedMatrix.GetLength(1) - 1)
        {
            if (augmentedMatrix[currentRow, currentColumn].IsZero)
            {
                // Interchange with row that is not zero
                int row;

                for (row = currentRow + 1; row < augmentedMatrix.GetLength(0); row++)
                {
                    if (!augmentedMatrix[row, currentColumn].IsZero)
                    {
                        break;
                    }
                }

                if (row == augmentedMatrix.GetLength(0))
                {
                    // No nonzero value found in this column
                    currentColumn++;

                    continue;
                }

                for (int column = currentColumn; column < augmentedMatrix.GetLength(1); column++)
                {
                    (augmentedMatrix[currentRow, column], augmentedMatrix[row, column]) = (augmentedMatrix[row, column], augmentedMatrix[currentRow, column]);
                }
            }

            // Store pivot location for later use
            pivots.Add((currentRow, currentColumn));

            // Make pivot one by Scaling
            var scale = augmentedMatrix[currentRow, currentColumn];

            for (int column = currentColumn; column < augmentedMatrix.GetLength(1); column++)
            {
                augmentedMatrix[currentRow, column] /= scale;
            }

            // Make column below pivot zero by Replacement
            for (int row = currentRow + 1; row < augmentedMatrix.GetLength(0); row++)
            {
                scale = augmentedMatrix[row, currentColumn];

                for (int column = currentColumn; column < augmentedMatrix.GetLength(1); column++)
                {
                    augmentedMatrix[row, column] -= scale * augmentedMatrix[currentRow, column];
                }
            }

            // Move to next row/column 
            currentRow++;
            currentColumn++;
        }

        // Check that result of remaining rows is zero
        while (currentRow < augmentedMatrix.GetLength(0))
        {
            if (!augmentedMatrix[currentRow, augmentedMatrix.GetLength(1) - 1].IsZero)
            {
                // The system has no solutions
                return null;
            }

            currentRow++;
        }

        // Create reduced echelon
        foreach ((var pivotRow, var pivotColumn) in pivots.Reverse<(int, int)>())
        {
            // Make column above pivot zero by Replacement
            for (int row = 0; row < pivotRow; row++)
            {
                var scale = augmentedMatrix[row, pivotColumn];

                for (int column = pivotColumn; column < augmentedMatrix.GetLength(1); column++)
                {
                    augmentedMatrix[row, column] -= scale * augmentedMatrix[pivotRow, column];
                }
            }
        }

        // Extract result
        var solution = new Quotient[augmentedMatrix.GetLength(0)];

        for (int row = 0; row < augmentedMatrix.GetLength(0); row++)
        {
            solution[row] = augmentedMatrix[row, augmentedMatrix.GetLength(1) - 1];
        }

        return solution;
    }
}
