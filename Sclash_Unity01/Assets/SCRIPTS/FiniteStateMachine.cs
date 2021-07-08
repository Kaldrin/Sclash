using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fsm
{
    class FiniteStateMachine
    {
        public enum States { Wait, Attack, Parry, Pommel, DashToward, DashAway, InterruptAttack };
        public States State { get; set; }

        public enum Events { };

        private Action[,] fsm;

        public FiniteStateMachine()
        {
            this.fsm = new Action[7, 4]
            {
            {null,null,null,null},
            {null,null,null,null},
            {null,null,null,null},
            {null,null,null,null},
            {null,null,null,null},
            {null,null,null,null},
            {null,null,null,null}
            };
        }

        public void ProcessEvent(Events theEvent)
        {
            this.fsm[(int)this.State, (int)theEvent].Invoke();
        }
    }
}
