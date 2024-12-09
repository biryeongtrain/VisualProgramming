using System.ComponentModel;

namespace App2.Logic;

/// <summary>
/// 컴포넌트를 제작하는 클래스
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class ComponentFactory<T>
{
    public abstract T CreateComponent();

}