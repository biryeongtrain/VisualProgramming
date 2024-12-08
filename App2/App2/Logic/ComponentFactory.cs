using System.ComponentModel;

namespace App2.Logic;

public abstract class ComponentFactory<T>
{
    public abstract T CreateComponent();

}