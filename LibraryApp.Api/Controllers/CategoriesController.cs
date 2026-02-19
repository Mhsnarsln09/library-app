// [HttpPost]
// public async Task<ActionResult<CategoryDetailDto>> Create(
//     CreateCategoryDto dto,
//     CreateCategory useCase,
//     CancellationToken ct)
// {
//     try
//     {
//         var category = await useCase.HandleAsync(dto.Name, ct);
//         return CreatedAtAction(nameof(GetById), new { id = category.Id },
//             new CategoryDetailDto(category.Id, category.Name));
//     }
//     catch (ArgumentException ex)
//     {
//         return BadRequest(ex.Message);
//     }
// }
