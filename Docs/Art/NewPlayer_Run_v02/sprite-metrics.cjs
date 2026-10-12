function face(d,w,h){
 const seen=new Uint8Array(w*h),queue=new Int32Array(w*h),parts=[];
 const yes=p=>d[p*4+3]>128&&d[p*4]>220&&d[p*4+1]>175&&d[p*4+2]>105;
 for(let p=0;p<w*h;p++)if(!seen[p]&&yes(p)){
  let a=0,b=1,sx=0,sy=0;seen[p]=1;queue[0]=p;
  while(a<b){const v=queue[a++],x=v%w,y=Math.floor(v/w);sx+=x;sy+=y;
   for(let yy=Math.max(0,y-1);yy<=Math.min(h-1,y+1);yy++)for(let xx=Math.max(0,x-1);xx<=Math.min(w-1,x+1);xx++){const q=yy*w+xx;if(!seen[q]&&yes(q)){seen[q]=1;queue[b++]=q;}}
  }
  if(b>500)parts.push({area:b,x:sx/b,y:sy/b});
 }return parts.sort((a,b)=>b.area-a.area)[0];
}
// Sprite separation: select the dominant connected alpha silhouette in each
// sheet cell, excluding neighboring-row fragments and detached source speckles.
// Preserve RGB and the original soft alpha on that silhouette and its 2px edge.
function extractBody(raw,w,h){
 const seen=new Uint8Array(w*h),queue=new Int32Array(w*h);let largest=[];
 for(let p=0;p<w*h;p++)if(!seen[p]&&raw[p*4+3]>16){
  let a=0,b=1;seen[p]=1;queue[0]=p;
  while(a<b){const v=queue[a++],x=v%w,y=Math.floor(v/w);
   for(let yy=Math.max(0,y-1);yy<=Math.min(h-1,y+1);yy++)for(let xx=Math.max(0,x-1);xx<=Math.min(w-1,x+1);xx++){const q=yy*w+xx;if(!seen[q]&&raw[q*4+3]>16){seen[q]=1;queue[b++]=q;}}
  }if(b>largest.length)largest=Array.from(queue.subarray(0,b));
 }
 if(largest.length<1000)throw Error('Dominant sprite silhouette missing');
 const keep=new Uint8Array(w*h);
 for(const v of largest){const x=v%w,y=Math.floor(v/w);for(let yy=Math.max(0,y-2);yy<=Math.min(h-1,y+2);yy++)for(let xx=Math.max(0,x-2);xx<=Math.min(w-1,x+2);xx++)keep[yy*w+xx]=1;}
 const out=Buffer.from(raw);for(let p=0;p<w*h;p++)if(!keep[p])out.fill(0,p*4,p*4+4);
 return out;
}

module.exports={face,extractBody};
