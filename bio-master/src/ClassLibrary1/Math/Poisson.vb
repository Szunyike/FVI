Imports System.Math
Namespace Szunyi

    Public Class PoissonEvaluator
        Public Property lambda As Decimal
        Public Function x()

        End Function
        Public Sub New(Optional lambda As Decimal = 1D)
                Me.lambda = lambda
            End Sub

            Public Function ProbabilityMassFunction(k As Integer) As Decimal
                '(l^k / k! ) * e^-l
                'l = lamda
                Dim kFactorial As Integer = Factorial(k)
            Dim numerator As Double = Pow(E, -CDbl(lambda)) * Pow(CDbl(lambda), CDbl(k))

            Dim p As Decimal = CDec(numerator) / kFactorial
                Return p
            End Function

            Public Function CummulitiveDistributionFunction(k As Integer) As Decimal
            Dim e As Double = Pow(e, CDbl(-lambda))
            Dim i As Integer = 0
                Dim sum As Double = 0.0
                While i <= k
                Dim n As Double = Pow(CDbl(lambda), i) / Factorial(i)
                sum += n
                    i += 1
                End While
                Dim cdf As Decimal = CDec(e) * CDec(sum)
                Return cdf
            End Function

            Private Function Factorial(k As Integer) As Integer
                Dim count As Integer = k
                Dim factorial__1 As Integer = 1
                While count >= 1
                '     factorial__1 = factorial__1 * count
                count -= 1
            End While
                Return factorial__1
            End Function
        End Class
    End Namespace


