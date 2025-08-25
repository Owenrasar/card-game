def bubble_sort(list_):
    n = len(list_)
    for i in range(n):
        for j in range(0, n-i-1):
            if list_[j][1] < list_[j+1][1] :
                list_[j], list_[j+1] = list_[j+1], list_[j]

my_list = [["GPT", 100], ["Codee", 120], ["Ada", 110], ["Turing", 130], ["Hopper", 90], ["Babbage", 140]]
bubble_sort(my_list)

print ("Sorted list:")
for i in range(len(my_list)): # Goes through each item in the list
	name = my_list[i][0] # Grabs the name
	score = my_list[i][1] # Grabs the score
	print (f"Name: {name} Score: {score}") # Prints the information in the desired format