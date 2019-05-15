﻿using System;
using System.Collections.Generic;
using FluidHTN.Compounds;
using FluidHTN.PrimitiveTasks;

namespace FluidHTN
{
    public static class DomainBuilderExt
    {
        // ========================================================= COMPOUND TASKS

        /// <summary>
        ///     A compound task that will pick a random sub-task to decompose.
        ///     Sub-tasks can be sequences, selectors or actions.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DB RandomSelect<DB, T>(this DB builder, string name)
            where DB : BaseDomainBuilder<DB, T>
            where T : IContext
        {
            return builder.CompoundTask<RandomSelector>(name);
        }

        /// <summary>
        ///     A compound task that will pick the sub-task with the highest utility score.
        ///     Sub-tasks can be sequences, selectors or actions that implement the IUtilityTask interface.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DB UtilitySelect<DB, T>(this DB builder, string name)
            where DB : BaseDomainBuilder<DB, T>
            where T : IContext
        {
            return builder.CompoundTask<UtilitySelector>(name);
        }

        /// <summary>
        ///     A compound task that will find the shortest path through a sequence of sub-tasks
        ///     in order to reach the defined goal state.
        ///     Sub-tasks can only be primitive tasks that implement the IGOAPTask interface.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DB GOAPSequence<DB, T>(this DB builder, string name, params KeyValuePair<byte, byte>[] goal)
            where DB : BaseDomainBuilder<DB, T>
            where T : IContext
        {
            builder.CompoundTask<GOAPSequence>(name);
            if (builder.Pointer is GOAPSequence goap)
            {
                foreach (var kvp in goal)
                {
                    goap.AddGoalState(kvp.Key, kvp.Value);
                }
            }

            return builder;
        }

        // ========================================================= PRIMITIVE TASKS

        /// <summary>
        ///     A primitive task that implements the Utility Task interface, for use as sub-task of the
        ///     UtilitySelect compound task.
        ///     This primitive task can contain conditions, an operator and effects.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DB UtilityAction<DB, T, P>(this DB builder, string name)
            where DB : BaseDomainBuilder<DB, T>
            where T : IContext
            where P : IUtilityTask, IPrimitiveTask, new()
        {
            if (builder.Pointer is UtilitySelector)
            {
                return builder.PrimitiveTask<P>(name);
            }
            else
            {
                throw new Exception("Pointer is not a Utility Selector, which is required for adding Utility Actions!");
            }
        }

        /// <summary>
        ///     A primitive task that implements the GOAP Task interface, for use as sub-task of the
        ///     GOAP Sequence compound task.
        ///     This primitive task can contain conditions, an operator and effects.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DB GOAPAction<DB, T, P>(this DB builder, string name)
            where DB : BaseDomainBuilder<DB, T>
            where T : IContext
            where P : IGOAPTask, new()
        {
            if (builder.Pointer is GOAPSequence)
            {
                return builder.PrimitiveTask<P>(name);
            }
            else
            {
                throw new Exception("Pointer is not a GOAP Sequence, which is required for adding GOAP Actions!");
            }
        }

        // ========================================================= SERIALIZATION

        /// <summary>
        ///     Builds the designed domain and saves it to a json file, then returns the domain instance.
        /// </summary>
        /// <param name="fileName"></param>
        public static Domain<T> BuildAndSave<DB, T>(this DB builder, string fileName)
            where DB : BaseDomainBuilder<DB, T>
            where T : IContext
        {
            var domain = builder.Build();
            domain.Save(fileName);
            return domain;
        }

        /// <summary>
        ///     Loads a designed domain from a json file and returns a domain instance of it.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static Domain<T> Load<DB, T>(this DB builder, string fileName)
            where DB : BaseDomainBuilder<DB, T>
            where T : IContext
        {
            var domain = new Domain<T>(string.Empty);
            domain.Load(fileName);
            return domain;
        }
    }
}