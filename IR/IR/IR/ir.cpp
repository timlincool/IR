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



int main()
{
//開檔
FILE *fptr;
FILE *fptr1;
int count=0;
int count1=0;
char cbuff[50],**info;
char cbuff1[50],**info1;
fptr = fopen("c:\\AssessmentTrainSet.txt", "r");
  // fptr = fopen("C:\\123456.txt", "r");
	//fptr1 = fopen("C:\\1234567.txt", "r");
		fptr1 = fopen("c:\\ResultsTrainSetrelevent.txt", "r");
    if (!fptr) {
        printf("檔案開啟失敗...\n");
        exit(1);
    }
        if (!fptr1) {
        printf("檔案開啟失敗...\n");
        exit(1);
    }
	while(fscanf(fptr, "%s",cbuff)!=EOF)
	{
       count++; 
	}

	while(fscanf(fptr1, "%s",cbuff1)!=EOF)
	{

       count1++; 
	}
	info=new char *[count];
	info1=new char *[count1];
	for(int i=0;i<count;i++)
	{
      info[i]=new char[50];
	}
		
	for(int j=0;j<count1;j++)
	{
      info1[j]=new char[50];
	}
    fseek( fptr , 0 , SEEK_SET );
	fseek( fptr1 , 0 , SEEK_SET );

	for(int i=0;fscanf(fptr, "%s\n",info[i])!=EOF;i++)
	{
       // printf("info[%d]=%s\n",i,info[i]);   
	}
for(int j=0;fscanf(fptr1, "%s\n",info1[j])!=EOF;j++)
	{
       // printf("info1[%d]=%s\n",j,info1[j]);   
	}

int QueryAdd[16]={95 ,13, 10 ,5 ,87, 3 ,2 ,35, 3 ,84, 31, 70, 4 ,13 ,9 ,4};


//Q1=95 13 10 5 87 Q6=3 2 35 3 84 Q11=31 70 4 13 9 4 每類數量
//讀進檔後分１６群
int query[20][100];
int temp=0;
int queryall[20]={0};
for(int i=0;i<16;i++)
{
temp+=QueryAdd[i];
queryall[i] = temp;
for(int p=(4*(i+1)+queryall[i]-QueryAdd[i]);p<(4*(i+1)+queryall[i]);p++)
for(int k=(4+(4+2265*2)*i);k<(4+(4+2265*2)*i+2265*2);k++)
{
int nstrcmp=strcmp(info[p],info1[k]);

if (nstrcmp==0)
{

query[i+1][p-(4*(i+1)+queryall[i]-QueryAdd[i]-1)]=(k-(4+(4+2265*2)*i-2))/2;
//printf("Query1= %d配對%d\n",p-(4-1),(k-(4-2))/2);
//printf("Query%d= %d配對%d\n",i+1,p-(4*(i+1)+queryall-QueryAdd[i]-1),(k-(4+(4+2265*2)*i-2))/2);
}
}
}



for(int i=1;i<=16;i++)//由小到大排序
sort(query[i],query[i]+QueryAdd[i-1]+1);


//int **ptr=&query;
//int **ptr=(int**)query;


float **max= new float*[20]; //用兩顆星new一牌一棵星
	 //*square=new int[N+1];
      
        int i = 0; 

		for(i=0;i<20;i++)
		{
          max[i]=new float[20];//一牌 new 一列
	  for(int j = 0;j<20;j++)
          max[i][j]= 0;
		}




/*
int QueryAdd[16]={95 ,13, 10 ,5 ,87, 3 ,2 ,35, 3 ,84, 31, 70, 4 ,13 ,9 ,4};
for (int i=1;i<=16;i++)
for(int j=1;j<=QueryAdd[i-1];j++)
{
	printf("query[%d]=%d\t",i,query[i][j]);

}
*/

PR1(query,16,max);//Interpolated Recall-Precision Curve
mAP(query,16 );//use Mean Average Precision
//float even[11]={0.0},add[11]={0.0};

/*for (int i=0;i<=16;i++)
  for(int j=0;j<=10;i++)
  {
	  //add[j]+= max[i][j];
     // even[j]=add[j]/16;
      printf("%f",max[i][j]);
}*/


fclose(fptr);
  fclose(fptr1);
 system("pause");
 return 0;


}