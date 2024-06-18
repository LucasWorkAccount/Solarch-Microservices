namespace Medical_Record_System;

public interface IEventHandler
{
    Task Apply(Event @event);
}