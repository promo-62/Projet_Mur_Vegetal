

function myFunction() {
    
    var a = document.getElementById("a");
    var b = document.getElementById("b");
    var c = document.getElementById("c");
    a.style.display = "none";
    b.style.display = "none";
    c.style.display = "none";
    
    if (a.style.display === "none") {
      a.style.display = "block";
      b.style.display = "none";
      c.style.display = "none";
    } else if(b === "none"){
      a.style.display = "none";
      c.style.display = "none";
      b.style.display = "block";
    }
    else if (c === "none") {
      a.style.display = "none";
      b.style.display = "none";
      c.style.display = "block";
    }
   
  } 
  
      
    setInterval(() => {
            myFunction();
        }, 2000);
        
      
  

   
 