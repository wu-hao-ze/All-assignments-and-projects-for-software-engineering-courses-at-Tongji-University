/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Unity Technologies.
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Threading;

namespace Microsoft.Unity.VisualStudio.Editor
{
	internal class AsyncOperation<T>
	{
		private readonly Func<T> _producer;
		private readonly Func<Exception, T> _exceptionHandler;
		private readonly Action _finalHandler;
		private readonly ManualResetEventSlim _resetEvent;

		private T _result;
		private Exception _exception;

		private AsyncOperation(Func<T> producer, Func<Exception, T> exceptionHandler, Action finalHandler)
		{
			_producer = producer;
			_exceptionHandler = exceptionHandler;
			_finalHandler = finalHandler;
			_resetEvent = new ManualResetEventSlim(initialState: false);
		}

		public static AsyncOperation<T> Run(Func<T> producer, Func<Exception, T> exceptionHandler = null, Action finalHandler = null)
		{
			var task = new AsyncOperation<T>(producer, exceptionHandler, finalHandler);
			task.Run();
			return task;
		}

		private void Run()
		{
			ThreadPool.QueueUserWorkItem(_ =>
			{
				try
				{
					_result = _producer();
				}
				catch (Exception e)
				{
					_exception = e;

					if (_exceptionHandler != null)
					{
						_result = _exceptionHandler(e);
					}
				}
				finally
				{
					_finalHandler?.Invoke();
					_resetEvent.Set();
				}
			});
		}

		private void CheckCompletion()
		{
			if (!_resetEvent.IsSet)
				_resetEvent.Wait();
		}


		public T Result
		{
			get
			{
				CheckCompletion();
				return _result;
			}
		}

		public Exception Exception
		{
			get
			{
				CheckCompletion();
				return _exception;
			}
		}
	}
}
