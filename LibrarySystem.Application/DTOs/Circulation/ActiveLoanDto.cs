using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain.entities;
using LibrarySystem.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LibrarySystem.Application.DTOs.Circulation;

public class ActiveLoanDto
{
    public int RecordId { get; set; }
    public string CopyBarcode { get; set; } = string.Empty;
    public string ItemTitle { get; set; } = string.Empty;
    public string PatronName { get; set; } = string.Empty;    public DateTime BorrowDate { get; set; }
    public DateTime DueDate { get; set; }
    public bool IsOverdue => DueDate < DateTime.UtcNow;
}