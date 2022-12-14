using System;
using System.Collections.Generic;
using UnityEngine;

internal class UnityThreadDispatcher : MonoBehaviour
{
    internal static UnityThreadDispatcher wkr;
    internal static Queue<Action> jobs = new Queue<Action>();

    void Awake() {
        wkr = this;
    }

    void Update() {
        while (jobs.Count > 0) 
            jobs.Dequeue().Invoke();
    }

    internal void AddJob(Action newJob) {
        jobs.Enqueue(newJob);
    }
}