using NDomain.CQRS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDomain.Tests.Sample
{
   public class CounterIncremented
   {
      public int Increment { get; set; }
   }

   // reset, in past tense..
   public class CounterResetEvent { }

   public class CounterMultipliedEvent
   {
      public int Factor { get; set; }
   }

   //public class Counter : Aggregate
   //{
   //   public int Value { get { return this.State.Value; } }

   //   public void Increment(int increment = 1)
   //   {
   //      On(new CounterIncrementedEvent { Increment = increment });
   //   }

   //   public void Multiply(int factor)
   //   {
   //      On(new CounterMultipliedEvent { Factor = factor });
   //   }

   //   public void Reset()
   //   {
   //      On(new CounterResetEvent());
   //   }
   //}
}