using System;
using Prolog;

namespace Yodisoft.Test.Prolog
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            var prolog = new PrologEngine(persistentCommandHistory: false);
//            prolog.ConsultFromString(@"
//human(aaa).
//human(bbb).
//animals(ccc).
//droid(ddd).
//droid(eee).
//droid(ccc).
//life(X) :- human(X).
//life(X) :- animals(X).
//died(X) :- droid(X).
//kiborg(X) :- life(X), died(X).
//kiborg(kkk).
//");
            
            
            prolog.ConsultFromString(@"
parent(m1,m2).
parent(m2,m3).
parent(m3,m4).
parent(m4,m5).
all(X,Y):-parent(X,Y),X\=Y.
all(X,Y):-parent(X,A),parent(A,Y),X\=Y.
all(X,Y):-parent(X,A),parent(A,B),parent(B,Y),X\=Y.

main :- 
    setof([X, Y], all(X, Y), L),
    write(L),
    halt.
");

           //var solutionSet = prolog.GetAllSolutions(null, "kiborg(H)");
           var solutionSet = prolog.GetAllSolutions(null, "main");
            foreach (var s in solutionSet.NextSolution)
            {
                var ss = string.Empty;
                foreach (var v in s.NextVariable)
                {
                    ss += v.Value +"  ";
                }
                Console.WriteLine(ss);
            }
        }
    }
}
