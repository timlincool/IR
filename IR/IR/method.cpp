#include <cstdlib>
#include <stdio.h>
#include <stdlib.h>
//#include "common.h"
//#include "stdafx.h"
#include <cstring>
#include <fstream>
#include <iostream>
#include <algorithm>
#include "method.h"
using namespace std ;
using std::iostream;
using std::ifstream;
using std::string;
using std::endl;
using std::cout;
//WCHAR* CharToWCHAR(char*);

struct data
{
//float mp,mr;
	float mp[100];
	float mr[100];
};


//use Mean Average Precision
float** mAP(int matrix[][100],int x1 )
//void PR1(int **matrix,int x1,int y1 )
{
	struct data nonmap;
    struct data *ptr;

	//float max[20][20]={0.0};
	int QueryAdd[16]={95 ,13, 10 ,5 ,87, 3 ,2 ,35, 3 ,84, 31, 70, 4 ,13 ,9 ,4};
	float	mAP[20]={0.0},precision[20]={0.0},nonmAP=0.0,inonmAP=0.0,temp[20]={0.0};
	//float	mAP=0.0,precision=0.0,nonmAP=0.0,inonmAP=0.0,temp=0.0;
  for (int i=1;i<=x1;i++)
  {
     for (int j=1;j<=QueryAdd[i-1];j++)
     {

       precision[i]=(matrix[i][j]);
	   temp[i]+=(j/precision[i]);


      }
	
    	  mAP[i]=temp[i]/QueryAdd[i-1];
	  nonmAP+=mAP[i];

    
	
 // printf("%f\n",mAP[i]);

 }
//printf("%f\n",nonmAP);
  inonmAP=nonmAP/16;
  printf("mAP=%f\n",inonmAP);
 return 0;
}


//Interpolated Recall-Precision Curve
float** PR1(int matrix[][100],int x1, float **max )
//void PR1(int **matrix,int x1,int y1 )
{
	struct data prcom;
    struct data *ptr;

	//float max[20][20]={0.0};
	int QueryAdd[16]={95 ,13, 10 ,5 ,87, 3 ,2 ,35, 3 ,84, 31, 70, 4 ,13 ,9 ,4};
float max100=0,max90=0,max80=0,max70=0,max60=0,max50=0,max40=0,max30=0,max20=0,max10=0,max0=0,x;
  for (int i=1;i<=x1;i++)
 for (int j=1;j<=QueryAdd[i-1];j++)
 {
float	m_recall,m_precision;
	m_precision=100.0*j/matrix[i][j];
    m_recall=100.0*j/QueryAdd[i-1];
	prcom.mp[j]=m_precision;
    prcom.mr[j]=m_recall;
if(100.0==prcom.mr[j])
			 
  {
	  x=prcom.mp[j];
      max[i][10]=x;
 }

 else if(90.0<=prcom.mr[j] && prcom.mr[j]<100.0)
{
     x=prcom.mp[j];
  if (x>max[i][9])
	 max[i][9]=x; 
}
 else  if(80.0<=prcom.mr[j] && prcom.mr[j]<90.0)
{ 
     x=prcom.mp[j];
  if (x>max[i][8])
	 max[i][8]=x;
 }
 else if(70.0<=prcom.mr[j] && prcom.mr[j]<80.0)
{ 
     x=prcom.mp[j];
  if (x>max[i][7])
	 max[i][7]=x;
 }
 else if(60.0<=prcom.mr[j] && prcom.mr[j]<70.0)
{ 
     x=prcom.mp[j];
  if (x>max[i][6])
	 max[i][6]=x;
 }
  else if(50.0<=prcom.mr[j] && prcom.mr[j]<60.0)
{ 
     x=prcom.mp[j];
  if (x>max[i][5])
	 max[i][5]=x;
 }
  else  if(40.0<=prcom.mr[j] && prcom.mr[j]<50.0)
{ 
     x=prcom.mp[j];
  if (x>max[i][4])
	 max[i][4]=x;
 }
  else if(30.0<=prcom.mr[j] && prcom.mr[j]<40.0)
{ 
     x=prcom.mp[j];
  if (x>max[i][3])
	 max[i][3]=x;
 }
  else if(20.0<=prcom.mr[j] && prcom.mr[j]<30.0)
{ 
     x=prcom.mp[j];
  if (x>max[i][2])
	 max[i][2]=x;
 }
  else if(10.0<=prcom.mr[j] && prcom.mr[j]<20.0)
{ 
     x=prcom.mp[j];
  if (x>max[i][1])
	max[i][1]=x;
 }
  else if(0<=prcom.mr[j] && prcom.mr[j]<10.0)
{ 
     x=prcom.mp[j];
  if (x>max[i][0])
	 max[i][0]=x;
  }
 }

for(int i=1;i<=x1;i++)
	  for(int j=0;j<=10;j++)
		 

{

   if  (max[i][j]==0 )
  {	
	   if(max[i][j+1]!=0 )
	  max[i][j]=max[i][j+1];
	
  else if(max[i][j+2]!=0 )
	  max[i][j]=max[i][j+2];
	
	 else    if(max[i][j+3]!=0 )
	  max[i][j]=max[i][j+3];
	 else  if(max[i][j+4]!=0 )
	  max[i][j]=max[i][j+4];
	 else  if(max[i][j+5]!=0 )
	  max[i][j]=max[i][j+5];
	
	  
	  }
	  }
	  
/*for(int i=1;i<=x1;i++)
{
  printf("query%d\t",i);
  printf("(%.2f,0)\t",max[i][0]);
   printf("(%.2f,10)\t",max[i][1]);
    printf("(%.2f,20)\t",max[i][2]);
	 printf("(%.2f,30)\t",max[i][3]);
	  printf("(%.2f,40)\t",max[i][4]);
	   printf("(%.2f,50)\t",max[i][5]);
	    printf("(%.2f,60)\t",max[i][6]);
		 printf("(%.2f,70)\t",max[i][7]);
		  printf("(%.2f,80)\t",max[i][8]);
		   printf("(%.2f,90)\t",max[i][9]);
		    printf("(%.2f,100)\t",max[i][10]);
			printf("***********\n");
		 
}*/
float even[11]={0.0},add[11]={0.0};

printf("(P,R)計算為以下值\n");

for(int j=0;j<=10;j++)
{


for (int i=1;i<=16;i++)
   {
	  add[j]+= max[i][j];
     even[j]=add[j]/16;
} 
printf("(%f,%d)\t",even[j],j*10);
}

//for(int j=0;j<=10;j++)

//for(int i=x1;i<=x1;i++)
  //for(int j=0;j<=10;i++)
	
	  // a=max(prcom.mp[j],prcom.mp[j+1]);
printf("\n");
return max;
 }
