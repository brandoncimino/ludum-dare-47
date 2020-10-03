using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstroForeman : MonoBehaviour
{
    public List<GameObject> StationedAstronauts = new List<GameObject>();
    public List<GameObject> TasksToDo = new List<GameObject>();
    public List<GameObject> TasksInProgress = new List<GameObject>();
    //These probably don't need to be the entire GameObject

    public float resendTimer = 0;
    public float resendInterval = 3f;

    public void ReceiveTask(GameObject targetOfTask)
    {
        if (!TasksToDo.Contains(targetOfTask))
        {
            if(!SendTask(targetOfTask))
                TasksToDo.Add(targetOfTask);
        }
    }

    public bool SendTask(GameObject targetOfTask)
    {
        /*TODO: Check StationedAstronauts in order of closest to "targetOfTask" for first taskless Astronaut, assign the
         * task, and return true. If none are available, return false.*/
        return false;
    }

    public void TaskComplete(GameObject finishedTargetOfTask)
    {
        //TODO: Check StationedAstronauts for all Astronauts with this task and fire them.
        TasksInProgress.Remove(finishedTargetOfTask);
    }

    public void Update()
    {
        resendTimer += Time.deltaTime;
        if (resendTimer >= resendInterval)
        {
            resendTimer = 0;
            List<GameObject> AssignedTasks = new List<GameObject>();
            foreach (var VARIABLE in TasksToDo)
            {
                if (SendTask(VARIABLE))
                {
                    AssignedTasks.Add(VARIABLE);
                }
            }
            TasksToDo.RemoveAll(assigned => AssignedTasks.Contains(assigned));
        }
    }
}

//TODO: Tasks never enter TasksInProgress, but I'm too fucking tired, and I'm going to bed
//TODO: Create function to call if an Astronaut quits the Task (only if they die or someone else is there)