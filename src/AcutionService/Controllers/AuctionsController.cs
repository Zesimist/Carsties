using System;
using AcutionService.Data;
using AcutionService.DTOs;
using AcutionService.Entities;
using AutoMapper;
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
}
