using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain.entities;
using LibrarySystem.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LibrarySystem.Application.DTOs.ItemCopies;

public class ItemCopyResponse
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public string Barcode { get; set; } = string.Empty;
    public int Status { get; set; } 
    public string? Notes { get; set; }
}
