using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
	public decimal ListPrice { get; set; }

	[Required]
	[Precision(18, 2)]
	public decimal Price { get; set; }

	[Required]
	[Precision(18, 2)]
	public decimal Price50 { get; set; }

	[Required]
	[Precision(18, 2)]
	public decimal Price100 { get; set; }

	public string ImageUrl { get; set; }

	[Required]
	public int CategoryId { get; set; }

	[ForeignKey("CategoryId")]
	public Category Category { get; set; }

	[Required]
	public int CoverTypeId { get; set; }

	[ForeignKey("CoverTypeId")]
	public CoverType CoverType { get; set; }

}
