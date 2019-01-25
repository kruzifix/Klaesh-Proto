using System.Collections;
using UnityEngine;

namespace Klaesh
{
    public interface ICoroutineStarter
    {
        Coroutine StartCoroutine(IEnumerator method);
        void StopCoroutine(Coroutine routine);
    }
}
