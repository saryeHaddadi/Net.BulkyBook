using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BulkyBook.Models;

public class Product
{
	[Key]
	public int Id { get; set; }

	[Required]
	public string Title { get; set; }

	public string Description { get; set; }

	[Required]
	public string ISBN { get; set; }

	[Required]
	public string Author { get; set; }

	[Required]
	[Precision(18, 2)]
	[Display(Name = "List Price")]
	public decimal ListPrice { get; set; }

	[Required]
	[Precision(18, 2)]
	[Display(Name = "Price for 1-50")]
	public decimal Price { get; set; }

	[Required]
	[Precision(18, 2)]
	[Display(Name = "Price for 50-100")]
	public decimal Price50 { get; set; }

	[Required]
	[Precision(18, 2)]
	[Display(Name = "Price for 100+")]
	public decimal Price100 { get; set; }

	[ValidateNever]
	public string ImageUrl { get; set; }

	[Required]
	[Display(Name = "Category")]
	public int CategoryId { get; set; }

	[ForeignKey("CategoryId")]
	[ValidateNever]
	public Category Category { get; set; }

	[Required]
	[Display(Name = "Cover Type")]
	public int CoverTypeId { get; set; }

	[ForeignKey("CoverTypeId")]
	[ValidateNever]
	public CoverType CoverType { get; set; }

}
