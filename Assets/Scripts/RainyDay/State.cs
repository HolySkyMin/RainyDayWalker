using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RainyDay
{
    /// <summary>
    /// 상태 인터페이스입니다.
    /// </summary>
    public interface IState
    {
        /// <summary>
        /// 상태의 이름입니다.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 이 상태가 현재 상태로 등록된 순간 실행됩니다.
        /// </summary>
        void OnEnter();

        /// <summary>
        /// 이 상태가 더 이상 현재 상태가 아니게 된 순간 실행됩니다.
        /// </summary>
        void OnExit();

        /// <summary>
        /// 이 상태가 현재 상태일 때 Unity의 Update 함수에 맞춰 실행됩니다.
        /// </summary>
        void Update();

        /// <summary>
        /// 이 상태가 현재 상태일 때 Unity의 FixedUpdate 함수에 맞춰 실행됩니다.
        /// </summary>
        void FixedUpdate();

        /// <summary>
        /// 이 상태가 현재 상태일 때 Unity의 LateUpdate 함수에 맞춰 실행됩니다.
        /// </summary>
        void LateUpdate();
    }

    /// <summary>
    /// 상태 클래스입니다. 레퍼런스 홀딩할 오브젝트를 하나 받습니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class State<T> : IState
    {
        public string Name => _name;

        public StateMachine Machine => _machine;

        public T Parent => _parent;

        string _name;
        StateMachine _machine;
        T _parent;

        public State(string name, StateMachine machine, T parent)
        {
            _name = name;
            _machine = machine;
            _parent = parent;
        }

        public virtual void OnEnter() { }

        public virtual void OnExit() { }

        public virtual void Update() { }

        public virtual void FixedUpdate() { }

        public virtual void LateUpdate() { }
    }
}
