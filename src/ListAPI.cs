using Microsoft.AspNetCore.Mvc;

namespace NatrixServices;

public interface IHasId
{
    public string Id { get; }
}


[ApiController]
public abstract class ListAPI<T>(string BaseUrl) : ControllerBase where T : class, IHasId
{
    [HttpGet]
    public virtual async Task<IActionResult> GetAllItems()
    {
        var data = await GetData();
        if (data == null) return NotFound();

        return Ok(data);
    }

    [HttpPost]
    public virtual async Task<IActionResult> AddItem([FromBody] T obj)
    {
        var data = await GetData();
        if (data == null) return NotFound();

        data.Add(obj.Id, obj);
        await SaveChanges();

        Console.WriteLine($"Created object with id \"{obj.Id}\"");

        return Created($"{BaseUrl}/{obj.Id}", obj);
    }

    [HttpDelete("{itemId}")]
    public virtual async Task<IActionResult> RemoveItem(string itemId)
    {
        var data = await GetData();
        if (data == null) return NotFound();

        data.Remove(itemId);
        await SaveChanges();

        return Ok();
    }

    [HttpGet("{itemId}/{property}")]
    public virtual async Task<IActionResult> GetItemProperty(string itemId, string property)
    {
        var data = await GetData();
        if (data == null) return NotFound();

        if (!data.TryGetValue(itemId, out T? obj)) return NotFound("Item does not exist");

        object? value = GetItemProperty(property, obj, out string? error);

        if (error != null)
            return BadRequest(error);

        return Ok(value);
    }

    [HttpPatch("{itemId}/{property}")]
    public virtual async Task<IActionResult> PatchItemProperty(string itemId, string property, [FromBody] object newData)
    {
        var data = await GetData();
        if (data == null) return NotFound();

        if (!data.TryGetValue(itemId, out T? obj))
            return NotFound("Item does not exist");

        string? error = PatchItemProperty(property, obj, newData);

        if (error != null)
            return BadRequest(error);

        await SaveChanges();

        return Ok(obj);
    }

    protected abstract Task<Dictionary<string, T>?> GetData();
    protected abstract Task SaveChanges();
    protected abstract string? PatchItemProperty(string property, T obj, object newData);
    protected abstract object? GetItemProperty(string property, T obj, out string? error);
}