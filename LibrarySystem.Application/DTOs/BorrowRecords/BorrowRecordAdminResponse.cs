using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain.entities;
using LibrarySystem.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LibrarySystem.Application.DTOs.BorrowRecords;

public class BorrowRecordAdminResponse : BorrowRecordResponse
{
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
}