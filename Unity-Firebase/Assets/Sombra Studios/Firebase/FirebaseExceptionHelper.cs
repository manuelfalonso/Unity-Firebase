using Firebase;
using Firebase.Auth;
using System;
using System.Threading.Tasks;

/// <summary>
/// Method to return exception error message from a Firebase Excception
/// Known issue: Conflict with Task<T> de Unity.Tasks
/// </summary>
public class FirebaseExceptionHelper
{
    /// <summary>
    /// Loop through the InnerException(s) presented by the AggregateException,
    /// and look for the suspected FirebaseException.
    /// </summary>
    /// <param name="task">Firebase Task</param>
    /// <returns>Error Message</returns>
    public static string GetErrorFromException(Task<FirebaseUser> task)
    {
        string error = string.Empty;
        AggregateException ex = task.Exception as AggregateException;
        if (ex != null)
        {
            FirebaseException fbEx = null;
            foreach (Exception e in ex.InnerExceptions)
            {
                fbEx = e.GetBaseException() as FirebaseException;
                if (fbEx != null)
                    break;
            }

            if (fbEx != null)
            {
                error = fbEx.Message;
            }
        }
        return error;
    }
}
