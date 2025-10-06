using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class Move : Node
{
    // all move flags and variables here
    public Player Player;
    
    // Slide vars
    // public float SlideTimer = 0.0f;
    // public float SlideTimerMax = 1.0f;
    // public Vector2 SlideVector = Vector2.Zero;
    // public float SlideSpeed = 10.0f;
    
    // Movement vars
     // public float CurrentSpeed = 5.0f;
     public float LerpSpeed = 15.0f;
     // public float CrouchDepth = -0.5f;
     // public float FreeLookTiltAmount = 8.0f;

     public static Dictionary<string, int> MovesPriority = new()
     {
         {"idle",1},
         {"run", 2},
         {"jump", 10} // don't edit so much when other actions are added
     };
     

     public static bool MovesPrioritySort(string a, string b)
     {
         if (MovesPriority[a] > MovesPriority[b])
         {
             return true;
         }
         return false;
     }
     
     public static void SortListString(List<String> list)
     {
         // just copied bubble sort cus I'm lazy
         int i, j;
         string temp;
         bool swapped;
         for (i = 0; i < list.Count - 1; i++) {
             swapped = false;
             for (j = 0; j < list.Count - i - 1; j++) {
                 if (!MovesPrioritySort(list[j], list[j + 1])) {
                    
                     // Swap arr[j] and arr[j+1]
                     temp = list[j];
                     list[j] = list[j + 1];
                     list[j + 1] = temp;
                     swapped = true;
                 }
             }
             // If no two elements were
             // swapped by inner loop, then break
             if (swapped == false)
                 break;
         }
     }
    

    public virtual string CheckRelevance(InputPackage input)
    {
        GD.PrintErr("implementation error");
        return "implementation error";
    }

    public virtual void Update(InputPackage input, double delta)
    {
        
    }

    public virtual void OnEnterState()
    {
        
    }

    public virtual void OnExitState()
    {
        
    }
}
