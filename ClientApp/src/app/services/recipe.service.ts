import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators'
import { Recipe } from "../model/recipe";

@Injectable()
export class RecipeService {

  constructor(public http: HttpClient) {

  }

  public getRecipes(query?: string): Observable<Recipe[]> {
    return this.http.get<Recipe[]>('api/recipes', { params: { query } });
  }

  public getRecipeFile(filePath: string) {
    return this.http.get(`api/recipes/${filePath}`, { responseType: 'blob' });
  }

  public addRecipe(recipe: Recipe) {
    return this.http.post(`api/recipes`, recipe);
  }

  public deleteRecipe(recipe: Recipe) {
    return this.http.delete(`api/recipes/${recipe.name}/${recipe.path}`);
  }

  public editRecipe(oldRecipe: Recipe, newRecipe: Recipe) {
    let recipes: Recipe[] = [oldRecipe, newRecipe];
    return this.http.put(`api/recipes`, recipes);
  }
}
