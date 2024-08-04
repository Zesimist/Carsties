using System;
using AcutionService.Data;
using AcutionService.DTOs;
using AcutionService.Entities;
using AutoMapper;
using AutoMapper.Configuration.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcutionService.Controllers;

[ApiController]
[Route("api/auctions")]
public class AuctionsController:ControllerBase
{
    private readonly AuctionDbContext _context;
    private readonly IMapper _mapper;

    public AuctionsController(AuctionDbContext context,IMapper mapper){
        _context = context;
        _mapper = mapper;
    }
    [HttpGet]
    public async Task<ActionResult<List<AuctionDTO>>> GetAllAuctions(){
        var auctions= await _context.Auctions
            .Include(x=>x.Item)
            .OrderBy(x=>x.Item.Make)
            .ToListAsync();

        return _mapper.Map<List<AuctionDTO>>(auctions);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDTO>> GetAuctionById(Guid id){
        var auctions= await _context.Auctions
            .Include(x=>x.Item)
            .FirstOrDefaultAsync(x=>x.Id==id);
            
        if(auctions ==null){ return NotFound();  }

        return _mapper.Map<AuctionDTO>(auctions);
    }
    [HttpPost]
    public async Task<ActionResult<AuctionDTO>> CreateAuction(CreateAuctionDTO auctionDto)
    {
        var auction = _mapper.Map<Auction>(auctionDto);
        //todo: add current user as seller
        auction.Seller ="test";
        _context.Auctions.Add(auction);
        var result = await _context.SaveChangesAsync() >0;
        if(!result) return BadRequest("could not save chnages to the db");

        return CreatedAtAction(nameof(GetAuctionById),new {auction.Id}, _mapper.Map<AuctionDTO>(auction));   
    }
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAuction(Guid id,UpdateAuctionDTO updateAuctionDto){
        var auction = _context.Auctions.Include(x=>x.Item)
                    .FirstOrDefaultAsync(x=>x.Id==id);
        
        if(auction==null){ return NotFound();}
        //Todo check seller ==username

        // auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
        // auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
        // auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
        // auction.Item.Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage;
        // auction.Item.Year = updateAuctionDto.Year ?? auction.Item.Year;

        var result = await _context.SaveChangesAsync() >0;

        if(result)  return Ok();

        return BadRequest("Problem saving changes");
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAuction(Guid id){
        var auction = _context.Auctions.FindAsync(id);

        if(auction ==null)  return NotFound();

        //todo checkseller ==username

        // _context.Auctions.Remove(auction);
        var result = await _context.SaveChangesAsync() >0;

        if(!result) return BadRequest("could not update DB");

        return Ok();
    }

}
