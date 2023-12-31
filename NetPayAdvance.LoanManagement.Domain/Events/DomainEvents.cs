﻿// using System;
// using System.Collections.Generic;
// using System.ComponentModel;
// using NetPayAdvance.LoanManagement.Domain.Abstractions;
//
// namespace NetPayAdvance.LoanManagement.Domain.Events;
//
// public static class DomainEvents
// {
//     [ThreadStatic]
//     private static List<Delegate> actions;
//
//     static DomainEvents()
//     {
//         Container = ;
//     }
//
//     public static IContainer Container { get; set; }
//     public static void Register<T>(Action<T> callback) where T : IDomainEvent
//     {
//         if (actions == null)
//         {
//             actions = new List<Delegate>();
//         }
//         actions.Add(callback);
//     }
//
//     public static void ClearCallbacks()
//     {
//         actions = null;
//     }
//
//     public static void Raise<T>(T args) where T : IDomainEvent
//     {
//         foreach (var handler in Container.GetAllInstances<IHandle<T>>())
//         {
//             handler.Handle(args);
//         }
//
//         if (actions != null)
//         {
//             foreach (var action in actions)
//             {
//                 if (action is Action<T>)
//                 {
//                     ((Action<T>)action)(args);
//                 }
//             }
//         }
//     }
// }