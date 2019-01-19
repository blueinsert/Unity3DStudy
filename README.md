# Unity3DStudy
介绍：

1.TinyCoroutineManager
   自己实现的简单的协程调度器，协程挂起指令实现了WaitForSeconds,WaitForFrames,WaitUntil,WaitWhile,WaitAny,WaitAll,Break等。不依赖MonoBehaviour(MonoBehaviour有个缺点：物体不可见后，协程调度也停止了，再次可见后也不会恢复协程), 简单的修改后(对时间查询的依赖)即可在别的c#环境中使用。
