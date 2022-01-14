using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models;

public class OrderHeader
{
	public int Id { get; set; }
	public string ApplicationUserId { get; set; }

	[ForeignKey(nameof(ApplicationUserId))]
	[ValidateNever]
	public ApplicationUser ApplicationUser { get; set; }
	[Required]
	public DateTime OrderDate { get; set; }
	[Required]
	public DateTime ShippingDate { get; set; }
	[Precision(18, 2)]
	public decimal OrderTotal { get; set; }
	public string? OrderStatus { get; set; }
	public string? PaymentStatus { get; set; }
	public string? TrackingNumber { get; set; }
	public string? Carrier { get; set; }
	public DateTime PaymentDate { get; set; }
	public DateTime PaymentDueDate { get; set; }
	public string? SessionId { get; set; }
	public string? PaymentIntentId { get; set; }
	[Required]
	public string Name { get; set; }
	[Required]
	public string StreetAdresse { get; set; }
	[Required]
	public string City { get; set; }
	[Required]
	public string State { get; set; }
	[Required]
	public string PostalCode { get; set; }
	[Required]
	public string PhoneNumber { get; set; }

}
