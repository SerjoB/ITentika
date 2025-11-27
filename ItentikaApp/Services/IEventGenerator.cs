using ItentikaApp.Data;
using ItentikaApp.Models;

namespace ItentikaApp.Services;

public interface IEventGenerator
{
    Event GenerateRandomEvent();
}