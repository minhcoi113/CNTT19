const toJson = (item) => {
    return {
        id : item.id,
        Title : item.Title ,
        StepId:item.StepId,
        ProjectId:item.ProjectId,
        Labels: item.Labels,
        DueDate: item.DueDate,
        Donvi: item.Donvi,
        Assignee: item.Assignee,
        Descrption: item.Descrption,
    }
}
const fromJson = (item) => {
    return {
        id : item.id,
        Title : item.Title ,
        StepId:item.StepId,
        ProjectId:item.ProjectId,
        Labels: item.Labels,
        DueDate: item.DueDate,
        Donvi: item.Donvi,
        Assignee: item.Assignee,
        Descrption: item.Descrption,
    }
}

const baseJson = () => {
    return {
        id : null,
        Title : null ,
        StepId: false,
        ProjectId: null,
        Labels: null,
        DueDate: null,
        Donvi: null,
        Assignee: null,
        Descrption: null,
    }
}



const toListModel = (items) =>{
    if(items.length > 0){
        let data = [];
        items.map((value, index) =>{
            data.push(fromJson(value));
        })
        return data??[];
    }
    return [];
}

export const yeucauloiModel = {
    toJson, fromJson, baseJson, toListModel
}
